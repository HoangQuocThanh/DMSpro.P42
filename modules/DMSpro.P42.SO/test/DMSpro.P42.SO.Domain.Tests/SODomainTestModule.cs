using DMSpro.P42.SO.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace DMSpro.P42.SO;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(SOEntityFrameworkCoreTestModule)
    )]
public class SODomainTestModule : AbpModule
{

}
