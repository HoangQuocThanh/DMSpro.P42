using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.MDM.EntityFrameworkCore;

[ConnectionStringName(MDMDbProperties.ConnectionStringName)]
public interface IMDMDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
