using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.SO.EntityFrameworkCore;

[ConnectionStringName(SODbProperties.ConnectionStringName)]
public interface ISODbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
