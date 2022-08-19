using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Volo.Abp.Account.Security.Recaptcha;
using Volo.Abp.Account.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Account;

[RemoteService(Name = AccountProPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AccountProPublicRemoteServiceConsts.ModuleName)]
[Route("api/account")]
public class AccountController : AbpControllerBase, IAccountAppService
{
    protected static byte[] DefaultAvatarContent = null;

    protected IAccountAppService AccountAppService { get; }

    protected IVirtualFileProvider VirtualFileProvider { get; }

    public IAbpRecaptchaValidatorFactory RecaptchaValidatorFactory { get; set; }

    protected IOptionsSnapshot<reCAPTCHAOptions> ReCaptchaOptions { get; }

    protected ISettingProvider SettingProvider { get; }

    public AccountController(
        IAccountAppService accountAppService,
        IVirtualFileProvider virtualFileProvider,
        IOptionsSnapshot<reCAPTCHAOptions> reCaptchaOptions,
        ISettingProvider settingProvider)
    {
        AccountAppService = accountAppService;
        VirtualFileProvider = virtualFileProvider;
        ReCaptchaOptions = reCaptchaOptions;
        SettingProvider = settingProvider;
        RecaptchaValidatorFactory = NullAbpRecaptchaValidatorFactory.Instance;
    }

    [HttpPost]
    [Route("register")]
    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        if (await UseCaptchaOnRegistration())
        {
            var reCaptchaVersion = await SettingProvider.GetAsync<int>(AccountSettingNames.Captcha.Version);
            await ReCaptchaOptions.SetAsync(reCaptchaVersion == 3 ? reCAPTCHAConsts.V3 : reCAPTCHAConsts.V2);
        }
        return await AccountAppService.RegisterAsync(input);
    }

    [HttpPost]
    [Route("send-password-reset-code")]
    public virtual Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        return AccountAppService.SendPasswordResetCodeAsync(input);
    }

    [HttpPost]
    [Route("reset-password")]
    public virtual Task ResetPasswordAsync(ResetPasswordDto input)
    {
        return AccountAppService.ResetPasswordAsync(input);
    }

    [HttpGet]
    [Route("confirmation-state")]
    public Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id)
    {
        return AccountAppService.GetConfirmationStateAsync(id);
    }

    [HttpPost]
    [Route("send-phone-number-confirmation-token")]
    public Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        return AccountAppService.SendPhoneNumberConfirmationTokenAsync(input);
    }

    [HttpPost]
    [Route("send-email-confirmation-token")]
    public Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input)
    {
        return AccountAppService.SendEmailConfirmationTokenAsync(input);
    }

    [HttpPost]
    [Route("confirm-phone-number")]
    public Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        return AccountAppService.ConfirmPhoneNumberAsync(input);
    }

    [HttpPost]
    [Route("confirm-email")]
    public Task ConfirmEmailAsync(ConfirmEmailInput input)
    {
        return AccountAppService.ConfirmEmailAsync(input);
    }

    [Authorize]
    [HttpPost]
    [Route("profile-picture")]
    public virtual async Task SetProfilePictureAsync(ProfilePictureInput input)
    {
        await AccountAppService.SetProfilePictureAsync(input);
    }

    [HttpGet]
    [Route("profile-picture/{id}")]
    public virtual async Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id)
    {
        return await AccountAppService.GetProfilePictureAsync(id);
    }

    [HttpGet]
    [Route("two-factor-providers")]
    public virtual Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        return AccountAppService.GetTwoFactorProvidersAsync(input);
    }

    [HttpPost]
    [Route("send-two-factor-code")]
    public virtual Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        return AccountAppService.SendTwoFactorCodeAsync(input);
    }

    [HttpGet]
    [Route("security-logs")]
    public Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync([FromQuery] GetIdentitySecurityLogListInput input)
    {
        return AccountAppService.GetSecurityLogListAsync(input);
    }

    [HttpGet]
    [Route("profile-picture-file/{id}")]
    public virtual async Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id)
    {
        return await AccountAppService.GetProfilePictureFileAsync(id);
    }

    [HttpGet]
    [Route("recaptcha-validate")]
    public virtual async Task Recaptcha(string captchaResponse)
    {
        var reCaptchaVersion = await SettingProvider.GetAsync<int>(AccountSettingNames.Captcha.Version);
        await ReCaptchaOptions.SetAsync(reCaptchaVersion == 3 ? reCAPTCHAConsts.V3 : reCAPTCHAConsts.V2);

        var reCaptchaValidator = await RecaptchaValidatorFactory.CreateAsync();
        await reCaptchaValidator.ValidateAsync(captchaResponse);
    }

    protected virtual async Task<bool> UseCaptchaOnRegistration()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration);
    }
}
