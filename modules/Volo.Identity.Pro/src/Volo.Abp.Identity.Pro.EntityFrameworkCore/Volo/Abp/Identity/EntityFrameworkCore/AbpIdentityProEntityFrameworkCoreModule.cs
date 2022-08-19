using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Volo.Abp.Identity.EntityFrameworkCore;

[DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule))]
public class AbpIdentityProEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<IdentityProDbContext>(options =>
        {
            options.ReplaceDbContext<IIdentityDbContext, IIdentityProDbContext>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        
    }
}
