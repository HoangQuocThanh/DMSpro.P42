using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(SOApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class SOHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(SOApplicationContractsModule).Assembly,
            SORemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SOHttpApiClientModule>();
        });
    }
}
