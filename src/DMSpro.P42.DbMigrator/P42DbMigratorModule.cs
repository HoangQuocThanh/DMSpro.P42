using DMSpro.P42.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace DMSpro.P42.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(P42EntityFrameworkCoreModule),
    typeof(P42ApplicationContractsModule)
)]
public class P42DbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
}
