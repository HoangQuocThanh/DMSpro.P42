using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DMSpro.P42.Localization;
using DMSpro.P42.MultiTenancy;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Gdpr;
using DMSpro.P42.SO;
using DMSpro.P42.MDM;
using DMSpro.P42.eRoute;

namespace DMSpro.P42;

[DependsOn(
    typeof(P42DomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpIdentityServerDomainModule),
    typeof(AbpPermissionManagementDomainIdentityServerModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(SaasDomainModule),
    typeof(TextTemplateManagementDomainModule),
    typeof(LeptonThemeManagementDomainModule),
    typeof(LanguageManagementDomainModule),
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpEmailingModule),
    typeof(AbpGdprDomainModule),
    typeof(BlobStoringDatabaseDomainModule)
    )]
[DependsOn(typeof(SODomainModule))]
    [DependsOn(typeof(MDMDomainModule))]
    [DependsOn(typeof(eRouteDomainModule))]
    public class P42DomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية", "ae"));
            options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish", "fi"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français", "fr"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak", "sk"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский", "ru"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe", "tr"));
            options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina", "si"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文", "cn"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文", "tw"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
            options.Languages.Add(new LanguageInfo("nl", "nl", "Dutch", "nl"));
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif

       
    }
}
