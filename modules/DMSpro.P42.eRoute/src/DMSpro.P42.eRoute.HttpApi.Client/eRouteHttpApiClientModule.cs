using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(eRouteApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class eRouteHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(eRouteApplicationContractsModule).Assembly,
            eRouteRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<eRouteHttpApiClientModule>();
        });
    }
}
