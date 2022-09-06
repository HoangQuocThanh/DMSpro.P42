using Localization.Resources.AbpUi;
using DMSpro.P42.eRoute.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(eRouteApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class eRouteHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(eRouteHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<eRouteResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
