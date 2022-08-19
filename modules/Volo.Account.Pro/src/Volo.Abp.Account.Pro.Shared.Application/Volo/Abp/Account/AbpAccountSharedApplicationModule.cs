using Volo.Abp.Application;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.UI.Navigation;

namespace Volo.Abp.Account;

[DependsOn(
    typeof(AbpAccountSharedApplicationContractsModule),
    typeof(AbpEmailingModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpUiNavigationModule),
    typeof(AbpJsonModule)
    )]
public class AbpAccountSharedApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        
    }
}
