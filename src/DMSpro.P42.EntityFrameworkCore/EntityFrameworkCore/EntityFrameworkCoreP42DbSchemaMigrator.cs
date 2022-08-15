using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DMSpro.P42.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace DMSpro.P42.EntityFrameworkCore;

public class EntityFrameworkCoreP42DbSchemaMigrator
    : IP42DbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreP42DbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope (connection string is dynamically resolved).
         */

        var dbContextType = _serviceProvider.GetRequiredService<ICurrentTenant>().IsAvailable
            ? typeof(P42TenantDbContext)
            : typeof(P42DbContext);

        await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
            .Database
            .MigrateAsync();
    }
}
