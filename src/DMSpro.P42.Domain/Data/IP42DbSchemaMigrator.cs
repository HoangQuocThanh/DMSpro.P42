using System.Threading.Tasks;

namespace DMSpro.P42.Data;

public interface IP42DbSchemaMigrator
{
    Task MigrateAsync();
}
