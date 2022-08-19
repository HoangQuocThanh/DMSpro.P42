using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Volo.Abp.Identity.EntityFrameworkCore;

public static class IdentityProDbContextModelCreatingExtensions
{
    public static void ConfigureIdentityPro(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.ConfigureIdentity();
        builder.TryConfigureObjectExtensions<IdentityProDbContext>();
    }
}
