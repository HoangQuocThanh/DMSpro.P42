using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.PhoneNumber;
using Volo.Abp.Account.Security.Recaptcha;
using Volo.Abp.Account.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Users;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Account;

public class AccountAppService : ApplicationService, IAccountAppService
{
    protected IIdentityRoleRepository RoleRepository { get; }
    protected IIdentitySecurityLogRepository SecurityLogRepository { get; }
    protected IdentityUserManager UserManager { get; }
    protected IAccountEmailer AccountEmailer { get; }
    protected IAccountPhoneService PhoneService { get; }
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    public IAbpRecaptchaValidatorFactory RecaptchaValidatorFactory { get; set; }

    protected ISettingManager SettingManager { get; }
    protected IBlobContainer<AccountProfilePictureContainer> AccountProfilePictureContainer { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public AccountAppService(
        IdentityUserManager userManager,
        IAccountEmailer accountEmailer,
        IAccountPhoneService phoneService,
        IIdentityRoleRepository roleRepository,
        IdentitySecurityLogManager identitySecurityLogManager,
        IBlobContainer<AccountProfilePictureContainer> accountProfilePictureContainer,
        ISettingManager settingManager,
        IOptions<IdentityOptions> identityOptions,
        IIdentitySecurityLogRepository securityLogRepository)
    {
        RoleRepository = roleRepository;
        IdentitySecurityLogManager = identitySecurityLogManager;
        UserManager = userManager;
        AccountEmailer = accountEmailer;
        PhoneService = phoneService;
        AccountProfilePictureContainer = accountProfilePictureContainer;
        SettingManager = settingManager;
        IdentityOptions = identityOptions;
        SecurityLogRepository = securityLogRepository;

        LocalizationResource = typeof(AccountResource);
        RecaptchaValidatorFactory = NullAbpRecaptchaValidatorFactory.Instance;
    }

    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        await CheckSelfRegistrationAsync();

        if (await UseCaptchaOnRegistration())
        {
            var reCaptchaValidator = await RecaptchaValidatorFactory.CreateAsync();
            await reCaptchaValidator.ValidateAsync(input.CaptchaResponse);
        }

        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

        if (!user.EmailConfirmed)
        {
            await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
        }

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        var user = await GetUserByEmail(input.Email);
        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        await AccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);
        (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    public async Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);

        return new IdentityUserConfirmationStateDto
        {
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public virtual async Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
        }

        CheckPhoneNumber(user);

        var token = await UserManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
        await PhoneService.SendConfirmationCodeAsync(user, token);
    }

    public async Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    protected virtual async Task SendEmailConfirmationTokenAsync(
        IdentityUser user,
        string applicationName,
        string returnUrl,
        string returnUrlHash)
    {
        var confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        await AccountEmailer.SendEmailConfirmationLinkAsync(user, confirmationToken, applicationName, returnUrl, returnUrlHash);
    }

    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        CheckPhoneNumber(user);

        (await UserManager.ChangePhoneNumberAsync(user, user.PhoneNumber, input.Token)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
        });
    }

    public virtual async Task ConfirmEmailAsync(ConfirmEmailInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (user.EmailConfirmed)
        {
            return;
        }

        (await UserManager.ConfirmEmailAsync(user, input.Token)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangeEmail
        });
    }

    [Authorize]
    public virtual async Task SetProfilePictureAsync(ProfilePictureInput input)
    {
        await SettingManager.SetForUserAsync(CurrentUser.GetId(), AccountSettingNames.ProfilePictureSource, input.Type.ToString());

        var userIdText = CurrentUser.GetId().ToString();

        if (input.Type != ProfilePictureType.Image)
        {
            if (await AccountProfilePictureContainer.ExistsAsync(userIdText))
            {
                await AccountProfilePictureContainer.DeleteAsync(userIdText);
            }
        }
        else
        {
            if (input.ImageContent == null)
            {
                throw new NoImageProvidedException();
            }

            await AccountProfilePictureContainer.SaveAsync(userIdText, input.ImageContent.GetStream(), true);
        }
    }

    public virtual async Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id)
    {
        var pictureSource = await SettingManager.GetOrNullForUserAsync(AccountSettingNames.ProfilePictureSource, id);

        if (pictureSource == ProfilePictureType.Gravatar.ToString())
        {
            var user = await UserManager.GetByIdAsync(id);
            var gravatar = $"https://secure.gravatar.com/avatar/{GetGravatarHash(user.Email)}";
            return new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Gravatar,
                Source = gravatar,
                FileContent = await GetAvatarFromAvatarAsync(gravatar)
            };
        }

        if (pictureSource == ProfilePictureType.Image.ToString() && await AccountProfilePictureContainer.ExistsAsync(id.ToString()))
        {
            return new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Image,
                FileContent = await AccountProfilePictureContainer.GetAllBytesAsync(id.ToString())
            };
        }

        return new ProfilePictureSourceDto
        {
            Type = ProfilePictureType.None,
            FileContent = await GetDefaultAvatarAsync()
        };
    }

    public virtual async Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id)
    {
        var picture = await GetProfilePictureAsync(id);
        return new RemoteStreamContent(new MemoryStream(picture.FileContent), contentType: "image/jpeg", disposeStream: true);
    }

    public virtual async Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            return (await UserManager.GetValidTwoFactorProvidersAsync(user)).ToList();
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    public virtual async Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            switch (input.Provider)
            {
                case "Email":
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                        await AccountEmailer.SendEmailSecurityCodeAsync(user, code);
                        return;
                    }
                case "Phone":
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        await PhoneService.SendSecurityCodeAsync(user, code);
                        return;
                    }
                default:
                    throw new UserFriendlyException(L["Volo.Account:UnsupportedTwoFactorProvider"]);
            }
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    [Authorize]
    public async Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync(GetIdentitySecurityLogListInput input)
    {
        var securityLogs = await SecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.Action,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId
        );

        var totalCount = await SecurityLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.Action,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId
        );

        var securityLogDtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(securityLogs);
        return new PagedResultDto<IdentitySecurityLogDto>(totalCount, securityLogDtos);
    }

    protected virtual string GetGravatarHash(string emailAddress)
    {
        var encodedPassword = new UTF8Encoding().GetBytes(emailAddress);

        var hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

        return BitConverter.ToString(hash)
            .Replace("-", string.Empty)
            .ToLower();
    }

    protected virtual async Task<IdentityUser> GetUserByEmail(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", email]);
        }

        return user;
    }

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
        {
            throw new UserFriendlyException(L["Volo.Account:SelfRegistrationDisabled"]);
        }
    }

    protected virtual void CheckPhoneNumber(IdentityUser user)
    {
        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            throw new BusinessException("Volo.Account:PhoneNumberEmpty");
        }
    }

    protected virtual async Task CheckIfPhoneNumberConfirmationEnabledAsync()
    {
        if (!await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation))
        {
            throw new BusinessException("Volo.Account:PhoneNumberConfirmationDisabled");
        }
    }

    protected virtual async Task<bool> UseCaptchaOnRegistration()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration);
    }

    //TODO: cache byte[]
    protected virtual async Task<byte[]> GetAvatarFromAvatarAsync(string url)
    {
        var httpclient = LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>().CreateClient();
        var responseMessage = await httpclient.GetAsync(url);
        return await responseMessage.Content.ReadAsByteArrayAsync();
    }

    //TODO: cache byte[]
    protected virtual async Task<byte[]> GetDefaultAvatarAsync()
    {
        var virtualFileProvider = LazyServiceProvider.LazyGetRequiredService<IVirtualFileProvider>();
        using (var stream = virtualFileProvider.GetFileInfo("/Volo/Abp/Account/ProfilePictures/avatar.jpg").CreateReadStream())
        {
            return await stream.GetAllBytesAsync();
        }
    }
}
