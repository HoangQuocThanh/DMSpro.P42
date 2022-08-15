using Localization.Resources.AbpUi;
using DMSpro.P42.SO.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(SOApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class SOHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SOHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<SOResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
