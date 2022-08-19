using Volo.Abp.Features;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpVirtualFileSystemModule),
    typeof(AbpLocalizationModule),
    typeof(AbpFeaturesModule),
    typeof(AbpSettingsModule)
    )]
public class AbpIdentityProDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<IdentityResource>()
                .AddVirtualJson("/Volo/Abp/Identity/Localization/DomainShared");
        });
    }
}

