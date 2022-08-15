using Localization.Resources.AbpUi;
using DMSpro.P42.MDM.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(MDMApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class MDMHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(MDMHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<MDMResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
