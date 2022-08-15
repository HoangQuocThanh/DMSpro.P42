using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(SODomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class SOApplicationContractsModule : AbpModule
{

}
