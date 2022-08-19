using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Volo.Abp.Account;

public interface IProfileAppService : IApplicationService
{
    Task<ProfileDto> GetAsync();

    Task<ProfileDto> UpdateAsync(UpdateProfileDto input);

    Task ChangePasswordAsync(ChangePasswordInput input);

    Task<bool> GetTwoFactorEnabledAsync();

    Task SetTwoFactorEnabledAsync(bool enabled);
}
