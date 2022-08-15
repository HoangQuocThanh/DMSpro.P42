using Microsoft.EntityFrameworkCore;

namespace DMSpro.P42.EntityFrameworkCore;

public class P42TenantDbContextFactory :
    P42DbContextFactoryBase<P42TenantDbContext>
{
    protected override P42TenantDbContext CreateDbContext(
        DbContextOptions<P42TenantDbContext> dbContextOptions)
    {
        return new P42TenantDbContext(dbContextOptions);
    }
}
