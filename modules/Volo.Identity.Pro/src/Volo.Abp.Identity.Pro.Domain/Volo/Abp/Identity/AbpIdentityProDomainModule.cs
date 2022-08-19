using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Gdpr;
using Volo.Abp.Identity.ExternalLoginProviders.Ldap;
using Volo.Abp.Identity.ExternalLoginProviders.OAuth;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Ldap;
using Volo.Abp.Ldap.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Volo.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityDomainModule),
    typeof(AbpIdentityProDomainSharedModule),
    typeof(AbpLdapModule),
    typeof(AbpCachingModule),
    typeof(AbpGdprAbstractionsModule)
)]
public class AbpIdentityProDomainModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IdentityBuilder>(builder =>
        {
            builder.AddUserValidator<MaxUserCountValidator>();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddScoped(typeof(IUserStore<IdentityUser>), provider => provider.GetService(typeof(IdentityProUserStore)));
        
        Configure<AbpIdentityOptions>(options =>
        {
            options.ExternalLoginProviders.Add<LdapExternalLoginProvider>(LdapExternalLoginProvider.Name);
            options.ExternalLoginProviders.Add<OAuthExternalLoginProvider>(OAuthExternalLoginProvider.Name);
        });

        context.Services.AddHttpClient(OAuthExternalLoginManager.HttpClientName);
        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<IdentityResource>()
                .AddBaseTypes(typeof(LdapResource));
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        
    }
}
