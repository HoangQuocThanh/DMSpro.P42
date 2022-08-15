using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DMSpro.P42.Data;

/* This is used if database provider does't define
 * IP42DbSchemaMigrator implementation.
 */
public class NullP42DbSchemaMigrator : IP42DbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
