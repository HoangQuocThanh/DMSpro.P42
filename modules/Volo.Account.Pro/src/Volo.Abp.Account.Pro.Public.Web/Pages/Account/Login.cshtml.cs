using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Volo.Abp.Account.Public.Web.Security.Recaptcha;
using Volo.Abp.Account.Security.Recaptcha;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.Account.Settings;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Reflection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

[DisableAuditing]
public class LoginModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    public bool IsLinkLogin { get; set; }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; }

    public bool EnableLocalLogin { get; set; }

    public bool IsSelfRegistrationEnabled { get; set; }

    public bool ShowCancelButton { get; set; }

    public bool UseCaptcha { get; set; }

    //TODO: Why there is an ExternalProviders if only the VisibleExternalProviders is used.
    public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; }
    public IEnumerable<ExternalProviderModel> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    protected readonly IAuthenticationSchemeProvider SchemeProvider;
    protected readonly AbpAccountOptions AccountOptions;
    protected readonly ICurrentPrincipalAccessor CurrentPrincipalAccessor;
    protected readonly IAbpRecaptchaValidatorFactory RecaptchaValidatorFactory;
    protected readonly IAccountExternalProviderAppService AccountExternalProviderAppService;

    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IAbpRecaptchaValidatorFactory recaptchaValidatorFactory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptions<IdentityOptions> identityOptions,
        IOptionsSnapshot<reCAPTCHAOptions> reCaptchaOptions)
    {
        SchemeProvider = schemeProvider;
        AccountExternalProviderAppService = accountExternalProviderAppService;
        AccountOptions = accountOptions.Value;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        IdentityOptions = identityOptions;
        RecaptchaValidatorFactory = recaptchaValidatorFactory;
        ReCaptchaOptions = reCaptchaOptions;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();

        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

        UseCaptcha = await UseCaptchaOnLoginAsync();

        IsLinkLogin = await VerifyLinkTokenAsync();
        if (IsLinkLogin)
        {
            if (CurrentUser.IsAuthenticated)
            {
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentitySecurityLogActionConsts.Logout
                });

                await SignInManager.SignOutAsync();

                return Redirect(HttpContext.Request.GetDisplayUrl());
            }
        }

        return Page();
    }

    [UnitOfWork] //TODO: Will be removed when we implement action filter
    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        try
        {
            await ReCaptchaVerification();
        }
        catch (UserFriendlyException e)
        {
            if (e is ScoreBelowThresholdException)
            {
                var onScoreBelowThresholdResult = OnRecaptchaScoreBelowThreshold();
                if (onScoreBelowThresholdResult != null)
                {
                    return await onScoreBelowThresholdResult;
                }
            }

            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        ValidateModel();

        await IdentityOptions.SetAsync();

        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

        await ReplaceEmailToUsernameOfInputIfNeeds();

        IsLinkLogin = await VerifyLinkTokenAsync();

        var result = await SignInManager.PasswordSignInAsync(
            LoginInput.UserNameOrEmailAddress,
            LoginInput.Password,
            LoginInput.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress
        });

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./SendSecurityCode", new {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash,
                rememberMe = LoginInput.RememberMe,
                linkUserId = LinkUserId,
                linkTenantId = LinkTenantId,
                linkToken = LinkToken
            });
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
            var notAllowedUser = await GetIdentityUser(LoginInput.UserNameOrEmailAddress);
            if (notAllowedUser.IsActive && await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
            {
                await StoreConfirmUser(notAllowedUser);
                return RedirectToPage("./ConfirmUser", new {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            Alerts.Danger(L["LoginIsNotAllowed"]);
            return Page();
        }

        if (!result.Succeeded)
        {
            Alerts.Danger(L["InvalidUserNameOrPassword"]);
            return Page();
        }

        var user = await GetIdentityUser(LoginInput.UserNameOrEmailAddress);

        if (IsLinkLogin)
        {
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

                return RedirectToPage("./LinkLogged", new {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash,
                    TargetLinkUserId = LinkUserId,
                    TargetLinkTenantId = LinkTenantId
                });
            }
        }

        return RedirectSafely(ReturnUrl, ReturnUrlHash);
    }

    protected virtual async Task<IdentityUser> GetIdentityUser(string userNameOrEmailAddress)
    {
        //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
            await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        Debug.Assert(user != null, nameof(user) + " != null");

        return user;
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetCreateLinkUser()
    {
        IsLinkLogin = await VerifyLinkTokenAsync();
        if (IsLinkLogin)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });

            await SignInManager.SignOutAsync();
        }

        return RedirectToPage("./Login", new {
            ReturnUrl = ReturnUrl,
            ReturnUrlHash = ReturnUrlHash,
            LinkUserId = LinkUserId,
            LinkTenantId = LinkTenantId,
            LinkToken = LinkToken
        });
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

    protected virtual async Task<List<ExternalProviderModel>> GetExternalProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();
        var externalProviders = await AccountExternalProviderAppService.GetAllAsync();

        var externalProviderModels = new List<ExternalProviderModel>();
        foreach (var scheme in schemes)
        {
            if (IsRemoteAuthenticationHandler(scheme, externalProviders) || scheme.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
            {
                externalProviderModels.Add(new ExternalProviderModel
                {
                    DisplayName = scheme.DisplayName,
                    AuthenticationScheme = scheme.Name,
                    Icon = AccountOptions.ExternalProviderIconMap.GetOrDefault(scheme.Name)
                });
            }
        }

        return externalProviderModels;
    }

    protected virtual bool IsRemoteAuthenticationHandler(AuthenticationScheme scheme, ExternalProviderDto externalProviders)
    {
        if (ReflectionHelper.IsAssignableToGenericType(scheme.HandlerType, typeof(RemoteAuthenticationHandler<>)))
        {
            var provider = externalProviders.Providers.FirstOrDefault(x => x.Name == scheme.Name);
            return provider == null || provider.Enabled;
        }

        return false;
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds()
    {
        if (!ValidationHelper.IsValidEmailAddress(LoginInput.UserNameOrEmailAddress))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        if (userByEmail == null)
        {
            return;
        }

        LoginInput.UserNameOrEmailAddress = userByEmail.UserName;
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
    {
        //TODO: Did not implemented Identity Server 4 sample for this method (see ExternalLoginCallback in Quickstart of IDS4 sample)
        /* Also did not implement these:
         * - Logout(string logoutId)
         */

        if (remoteError != null)
        {
            Logger.LogWarning($"External login callback error: {remoteError}");
            return RedirectToPage("./Login");
        }

        await IdentityOptions.SetAsync();

        var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
        }

        IsLinkLogin = await VerifyLinkTokenAsync();

        var result = await SignInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true
        );

        if (!result.Succeeded)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = "Login" + result
            });
        }

        if (result.IsLockedOut)
        {
            Logger.LogWarning($"Cannot proceed because user is locked out!");
            return RedirectToPage("./LockedOut", new {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        if (result.IsNotAllowed)
        {
            Logger.LogWarning($"External login callback error: User is Not Allowed!");

            var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user.IsActive)
            {
                await StoreConfirmUser(user);
                return RedirectToPage("./ConfirmUser", new {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            return RedirectToPage("./Login");
        }

        if (result.Succeeded)
        {
            var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (IsLinkLogin)
            {
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

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = result.ToIdentitySecurityLogAction(),
                UserName = user.UserName
            });

            return RedirectSafely(returnUrl, returnUrlHash);
        }

        //TODO: Handle other cases for result!

        var email = loginInfo.Principal.FindFirstValue(AbpClaimTypes.Email);
        if (email.IsNullOrWhiteSpace())
        {
            return RedirectToPage("./Register", new {
                IsExternalLogin = true,
                ExternalLoginAuthSchema = loginInfo.LoginProvider,
                ReturnUrl = returnUrl
            });
        }

        var externalUser = await UserManager.FindByEmailAsync(email);
        if (externalUser == null)
        {
            externalUser = await CreateExternalUserAsync(loginInfo);
        }
        else
        {
            if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
            {
                CheckIdentityErrors(await UserManager.AddLoginAsync(externalUser, loginInfo));
            }
        }

        if (await HasRequiredIdentitySettings())
        {
            Logger.LogWarning($"New external user is created but confirmation is required!");

            await StoreConfirmUser(externalUser);
            return RedirectToPage("./ConfirmUser", new {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        await SignInManager.SignInAsync(externalUser, false);

        if (IsLinkLogin)
        {
            using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(externalUser)))
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
                    UserName = externalUser.UserName,
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

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = externalUser.Name
        });

        return RedirectSafely(returnUrl, returnUrlHash);
    }

    protected virtual async Task<IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        await IdentityOptions.SetAsync();

        var emailAddress = info.Principal.FindFirstValue(AbpClaimTypes.Email);

        var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

        (await UserManager.CreateAsync(user)).CheckErrors();
        (await UserManager.SetEmailAsync(user, emailAddress)).CheckErrors();
        (await UserManager.AddLoginAsync(user, info)).CheckErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

        user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
        user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);

        var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
        if (!phoneNumber.IsNullOrWhiteSpace())
        {
            var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);
            user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
        }

        await UserManager.UpdateAsync(user);

        return user;
    }

    protected virtual async Task<bool> UseCaptchaOnLoginAsync()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnLogin);
    }

    protected virtual async Task ReCaptchaVerification()
    {
        UseCaptcha = await UseCaptchaOnLoginAsync();
        if (UseCaptcha)
        {
            var reCaptchaVersion = await SettingProvider.GetAsync<int>(AccountSettingNames.Captcha.Version);

            await ReCaptchaOptions.SetAsync(reCaptchaVersion == 3 ? reCAPTCHAConsts.V3 : reCAPTCHAConsts.V2);

            var recaptchaValidator = await RecaptchaValidatorFactory.CreateAsync();
            await recaptchaValidator.ValidateAsync(HttpContext.Request.Form[RecaptchaValidatorBase.RecaptchaResponseKey]);
        }
    }

    protected virtual async Task<bool> HasRequiredIdentitySettings()
    {
        var requireConfirmedEmail = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail);
        var requireConfirmedPhoneNumber = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber);
        return requireConfirmedEmail || requireConfirmedPhoneNumber;
    }

    protected virtual Task<IActionResult> OnRecaptchaScoreBelowThreshold()
    {
        return null;
    }

    protected virtual async Task<IActionResult> CheckLocalLoginAsync()
    {
        ExternalProviders = await GetExternalProviders();
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        if (!EnableLocalLogin && IsExternalLoginOnly && ExternalLoginScheme != null)
        {
            return await OnPostExternalLogin(ExternalLoginScheme);
        }

        if (!EnableLocalLogin)
        {
            Alerts.Warning(L["LocalLoginIsNotEnabled"]);
            return Page();
        }

        return null;
    }

    public class LoginInputModel
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ExternalProviderModel
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }

        public string Icon { get; set; }
    }
}
