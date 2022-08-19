using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Owl.reCAPTCHA.v3;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Settings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.Settings;

namespace Volo.Abp.Account.Public.Web.Security.Recaptcha;

[ExposeServices(typeof(RecaptchaValidatorV3))]
public class RecaptchaValidatorV3 : RecaptchaValidatorBase, ITransientDependency
{
    protected IreCAPTCHASiteVerifyV3 ReCAPTCHASiteVerifyV3 { get; }

    protected ISettingProvider SettingProvider { get; }

    public RecaptchaValidatorV3(
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<AccountResource> stringLocalizer,
        IJsonSerializer jsonSerializer,
        IreCAPTCHASiteVerifyV3 reCaptchaSiteVerifyV3,
        ISettingProvider settingProvider) :
        base(httpContextAccessor,
            stringLocalizer,
            jsonSerializer)
    {
        ReCAPTCHASiteVerifyV3 = reCaptchaSiteVerifyV3;
        SettingProvider = settingProvider;
    }

    public override async Task ValidateAsync(string captchaResponse)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new Exception("RecaptchaValidator should be used in a valid HTTP context!");
        }

        if (captchaResponse.IsNullOrEmpty())
        {
            throw new UserFriendlyException(StringLocalizer["CaptchaCanNotBeEmpty"]);
        }

        var response = await ReCAPTCHASiteVerifyV3.Verify(new reCAPTCHASiteVerifyRequest
        {
            Response = captchaResponse,
            RemoteIp = httpContext.Connection?.RemoteIpAddress?.ToString()
        });

        var score = await SettingProvider.GetAsync<double>(AccountSettingNames.Captcha.Score);
        if (!response.Success || response.Score < score)
        {
            Logger.LogError("google response: " + JsonSerializer.Serialize(response));
            throw new ScoreBelowThresholdException(StringLocalizer["ScoreBelowThreshold"]);
        }
    }
}
