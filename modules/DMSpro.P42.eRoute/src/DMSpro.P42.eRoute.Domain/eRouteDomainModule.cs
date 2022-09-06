using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(eRouteDomainSharedModule)
)]
public class eRouteDomainModule : AbpModule
{

}
