using DMSpro.P42.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace DMSpro.P42;

[DependsOn(
    typeof(P42EntityFrameworkCoreTestModule)
    )]
public class P42DomainTestModule : AbpModule
{

}
