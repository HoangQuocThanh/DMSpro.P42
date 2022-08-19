using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Volo.Abp.Identity;

public interface IIdentitySettingsAppService : IApplicationService
{
    Task<IdentitySettingsDto> GetAsync();

    Task UpdateAsync(IdentitySettingsDto input);

    Task<IdentityLdapSettingsDto> GetLdapAsync();

    Task UpdateLdapAsync(IdentityLdapSettingsDto input);
    
    Task<IdentityOAuthSettingsDto> GetOAuthAsync();

    Task UpdateOAuthAsync(IdentityOAuthSettingsDto input);
}
