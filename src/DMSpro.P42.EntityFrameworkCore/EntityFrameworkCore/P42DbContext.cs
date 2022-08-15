using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace DMSpro.P42.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class P42DbContext : P42DbContextBase<P42DbContext>
{
    public P42DbContext(DbContextOptions<P42DbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Both);

        base.OnModelCreating(builder);
    }
}
