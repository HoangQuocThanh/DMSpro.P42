using Microsoft.EntityFrameworkCore;

namespace DMSpro.P42.EntityFrameworkCore;

public class P42DbContextFactory :
    P42DbContextFactoryBase<P42DbContext>
{
    protected override P42DbContext CreateDbContext(
        DbContextOptions<P42DbContext> dbContextOptions)
    {
        return new P42DbContext(dbContextOptions);
    }
}
