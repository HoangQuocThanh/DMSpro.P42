using DMSpro.P42.MDM.Companies;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.MDM.EntityFrameworkCore;

[ConnectionStringName(MDMDbProperties.ConnectionStringName)]
public class MDMDbContext : AbpDbContext<MDMDbContext>, IMDMDbContext
{
    public DbSet<Company> Companies { get; set; }
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public MDMDbContext(DbContextOptions<MDMDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureMDM();
    }
}