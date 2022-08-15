using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace DMSpro.P42.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class P42TenantDbContext : P42DbContextBase<P42TenantDbContext>
{
    public P42TenantDbContext(DbContextOptions<P42TenantDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Tenant);

        base.OnModelCreating(builder);
    }
}
