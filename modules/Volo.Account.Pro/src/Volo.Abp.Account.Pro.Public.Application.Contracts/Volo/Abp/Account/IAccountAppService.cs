using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Identity;

namespace Volo.Abp.Account;

public interface IAccountAppService : IApplicationService
{
    Task<IdentityUserDto> RegisterAsync(RegisterDto input);

    Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

    Task ResetPasswordAsync(ResetPasswordDto input);

    Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id);

    Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input);

    Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input);

    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input);

    Task ConfirmEmailAsync(ConfirmEmailInput input);

    Task SetProfilePictureAsync(ProfilePictureInput input);

    Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id);

    Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id);

    Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input);

    Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input);

    Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync(GetIdentitySecurityLogListInput input);
}
