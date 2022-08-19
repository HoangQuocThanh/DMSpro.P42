using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Settings;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class VerifySecurityCodeModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string Provider { get; set; }

    [BindProperty]
    public string Code { get; set; }

    [BindProperty]
    public bool RememberBrowser { get; set; }

    public bool IsRememberBrowserEnabled { get; protected set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor;

    public VerifySecurityCodeModel(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    [UnitOfWork]
    public virtual async Task OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new UserFriendlyException(L["VerifySecurityCodeNotLoggedInErrorMessage"]);
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync());

        IsRememberBrowserEnabled = await IsRememberBrowserEnabledAsync();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync());

        await IdentityOptions.SetAsync();

        var result = await SignInManager.TwoFactorSignInAsync(
            Provider,
            Code,
            RememberMe,
            await IsRememberBrowserEnabledAsync() && RememberBrowser
        );

        if (result.Succeeded)
        {
            if (await VerifyLinkTokenAsync())
            {
                var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
                using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                {
                    await IdentityLinkUserAppService.LinkAsync(new LinkUserInput
                    {
                        UserId = LinkUserId.Value,
                        TenantId = LinkTenantId,
                        Token = LinkToken
                    });

                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = IdentityProSecurityLogActionConsts.LinkUser,
                        UserName = user.UserName,
                        ExtraProperties =
                            {
                                { IdentityProSecurityLogActionConsts.LinkTargetTenantId, LinkTenantId },
                                { IdentityProSecurityLogActionConsts.LinkTargetUserId, LinkUserId }
                            }
                    });

                    using (CurrentTenant.Change(LinkTenantId))
                    {
                        var targetUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                        using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(targetUser)))
                        {
                            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                            {
                                Identity = IdentitySecurityLogIdentityConsts.Identity,
                                Action = IdentityProSecurityLogActionConsts.LinkUser,
                                UserName = targetUser.UserName,
                                ExtraProperties =
                                    {
                                        { IdentityProSecurityLogActionConsts.LinkTargetTenantId, targetUser.TenantId },
                                        { IdentityProSecurityLogActionConsts.LinkTargetUserId, targetUser.Id }
                                    }
                            });
                        }
                    }
                }
            }

            return RedirectSafely(ReturnUrl, ReturnUrlHash);
        }

        if (result.IsLockedOut)
        {
            return RedirectToPage("./LockedOut", new {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        if (result.IsNotAllowed)
        {
            var notAllowedUser = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (notAllowedUser != null && notAllowedUser.IsActive)
            {
                await StoreConfirmUser(notAllowedUser);
                return RedirectToPage("./ConfirmUser", new {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            Alerts.Warning(L["LoginIsNotAllowed"]);
            return Page();
        }

        Alerts.Warning(L["InvalidSecurityCode"]);

        return Page();
    }

    protected virtual async Task<bool> VerifyLinkTokenAsync()
    {
        if (LinkToken.IsNullOrWhiteSpace() || LinkUserId == null)
        {
            return false;
        }

        return await IdentityLinkUserAppService.VerifyLinkTokenAsync(new VerifyLinkTokenInput
        {
            UserId = LinkUserId.Value,
            TenantId = LinkTenantId,
            Token = LinkToken
        });
    }


    protected virtual async Task<bool> IsRememberBrowserEnabledAsync()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled);
    }
}
