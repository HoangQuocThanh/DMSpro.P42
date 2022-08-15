using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace DMSpro.P42.MDM;

[DependsOn(
    typeof(MDMDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class MDMApplicationContractsModule : AbpModule
{

}
