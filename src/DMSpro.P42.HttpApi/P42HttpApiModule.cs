using Localization.Resources.AbpUi;
using DMSpro.P42.Localization;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Saas.Host;
using Volo.Abp.LeptonTheme;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Abp.Gdpr;
using DMSpro.P42.SO;
using DMSpro.P42.MDM;

namespace DMSpro.P42;

 [DependsOn(
    typeof(P42ApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAuditLoggingHttpApiModule),
    typeof(AbpIdentityServerHttpApiModule),
    typeof(AbpAccountAdminHttpApiModule),
    typeof(LanguageManagementHttpApiModule),
    typeof(SaasHostHttpApiModule),
    typeof(LeptonThemeManagementHttpApiModule),
    typeof(AbpGdprHttpApiModule),
    typeof(TextTemplateManagementHttpApiModule)
    )]
[DependsOn(typeof(SOHttpApiModule))]
    [DependsOn(typeof(MDMHttpApiModule))]
    public class P42HttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<P42Resource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
