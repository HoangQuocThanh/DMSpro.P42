using DMSpro.P42.MDM.Companies;
using Volo.Abp.Domain;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(MDMDomainSharedModule)
)]
public class MDMDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpClaimsServiceOptions>(options =>
        {
            options.RequestedClaims.AddRange(new[] { 
                CompanyConsts.CurrentCompany, 
                CompanyConsts.ListCompany 
            });
        });
    }
}
