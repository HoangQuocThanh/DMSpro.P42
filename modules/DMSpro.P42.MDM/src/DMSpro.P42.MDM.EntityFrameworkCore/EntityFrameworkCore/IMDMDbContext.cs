using DMSpro.P42.MDM.Companies;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.MDM.EntityFrameworkCore;

[ConnectionStringName(MDMDbProperties.ConnectionStringName)]
public interface IMDMDbContext : IEfCoreDbContext
{
    DbSet<Company> Companies { get; set; }
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}