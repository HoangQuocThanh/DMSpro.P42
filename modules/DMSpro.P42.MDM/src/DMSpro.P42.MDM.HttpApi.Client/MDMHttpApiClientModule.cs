using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(MDMApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class MDMHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MDMApplicationContractsModule).Assembly,
            MDMRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MDMHttpApiClientModule>();
        });
    }
}
