using Volo.Abp.Modularity;

namespace DMSpro.P42;

[DependsOn(
    typeof(P42ApplicationModule),
    typeof(P42DomainTestModule)
    )]
public class P42ApplicationTestModule : AbpModule
{

}
