using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(eRouteDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class eRouteApplicationContractsModule : AbpModule
{

}
