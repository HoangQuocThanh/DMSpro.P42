using System.Threading.Tasks;

namespace Volo.Abp.Identity.Settings;

public interface IOAuthSettingProvider
{
    Task<string> GetClientIdAsync();

    Task<string> GetClientSecretAsync();

    Task<string> GetAuthorityAsync();

    Task<string> GetScopeAsync();

    Task<bool> GetRequireHttpsMetadataAsync();
}