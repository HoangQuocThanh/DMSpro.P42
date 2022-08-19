using System.Threading.Tasks;

namespace Volo.Abp.Account.Public.Web.ExternalProviders;

public interface IPostConfigureAccountExternalProviderOptions<in TOptions>
    where TOptions : class, new()
{
    Task PostConfigureAsync(string name, TOptions options);
}
