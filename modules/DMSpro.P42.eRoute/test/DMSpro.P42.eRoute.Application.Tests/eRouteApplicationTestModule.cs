using Volo.Abp.Modularity;

namespace DMSpro.P42.eRoute;

[DependsOn(
    typeof(eRouteApplicationModule),
    typeof(eRouteDomainTestModule)
    )]
public class eRouteApplicationTestModule : AbpModule
{

}
