using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(eRouteDomainModule),
    typeof(eRouteApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class eRouteApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<eRouteApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<eRouteApplicationModule>(validate: true);
        });
    }
}
