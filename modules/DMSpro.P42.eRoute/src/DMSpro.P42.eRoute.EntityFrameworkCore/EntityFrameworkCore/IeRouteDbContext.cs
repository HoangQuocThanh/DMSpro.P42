using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.eRoute.EntityFrameworkCore;

[ConnectionStringName(eRouteDbProperties.ConnectionStringName)]
public interface IeRouteDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
