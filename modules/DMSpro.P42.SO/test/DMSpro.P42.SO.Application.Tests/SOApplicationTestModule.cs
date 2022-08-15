using Volo.Abp.Modularity;

namespace DMSpro.P42.SO;

[DependsOn(
    typeof(SOApplicationModule),
    typeof(SODomainTestModule)
    )]
public class SOApplicationTestModule : AbpModule
{

}
