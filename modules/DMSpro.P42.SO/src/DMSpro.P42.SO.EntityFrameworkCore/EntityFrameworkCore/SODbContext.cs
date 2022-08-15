using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DMSpro.P42.SO.EntityFrameworkCore;

[ConnectionStringName(SODbProperties.ConnectionStringName)]
public class SODbContext : AbpDbContext<SODbContext>, ISODbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public SODbContext(DbContextOptions<SODbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSO();
    }
}
