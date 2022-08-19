using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

[IgnoreAntiforgeryToken]
public class LinkLoginModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid SourceLinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? SourceLinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string SourceLinkToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid TargetLinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? TargetLinkTenantId { get; set; }

    public string LinkLoginDomain { get; set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected AbpAccountOptions AccountOptions { get; }

    protected ITenantStore TenantStore { get; }

    public LinkLoginModel(ICurrentPrincipalAccessor currentPrincipalAccessor, IOptions<AbpAccountOptions> accountOptions, ITenantStore tenantStore)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        AccountOptions = accountOptions.Value;
        TenantStore = tenantStore;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (AccountOptions.IsTenantMultiDomain && SourceLinkTenantId != TargetLinkTenantId && CurrentTenant.Id != TargetLinkTenantId)
        {
            var tenantInfo = new BasicTenantInfo(null, null);
            if (TargetLinkTenantId != null)
            {
                var tenantConfiguration = await TenantStore.FindAsync(TargetLinkTenantId.Value);
                tenantInfo = new BasicTenantInfo(tenantConfiguration.Id, tenantConfiguration.Name);
            }

            LinkLoginDomain = (await AccountOptions.GetTenantDomain(HttpContext, tenantInfo)).EnsureEndsWith('/') + "Account/LinkLogin";

            return Page();
        }

        if (TargetLinkUserId == CurrentUser.Id && TargetLinkTenantId == CurrentTenant.Id)
        {
            return RedirectSafely(ReturnUrl, ReturnUrlHash);
        }

        if (await IdentityLinkUserAppService.VerifyLinkLoginTokenAsync(new VerifyLinkLoginTokenInput()
        {
            UserId = SourceLinkUserId,
            TenantId = SourceLinkTenantId,
            Token = SourceLinkToken
        }))
        {
            using (CurrentTenant.Change(SourceLinkTenantId))
            {
                var sourceUser = await UserManager.GetByIdAsync(SourceLinkUserId);
                using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(sourceUser)))
                {
                    if (await IdentityLinkUserAppService.IsLinkedAsync(new IsLinkedInput
                    {
                        UserId = TargetLinkUserId,
                        TenantId = TargetLinkTenantId
                    }))
                    {
                        var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
                        await SignInManager.SignOutAsync();
                        using (CurrentTenant.Change(TargetLinkTenantId))
                        {
                            var targetUser = await UserManager.GetByIdAsync(TargetLinkUserId);
                            await SignInManager.SignInAsync(targetUser, isPersistent);
                        }
                        return RedirectSafely(ReturnUrl, ReturnUrlHash);
                    }
                }
            }
        }

        throw new UserFriendlyException(L["TheTargetAccountIsNotLinkedToYou"]);
    }
}
