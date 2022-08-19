using Volo.Abp.EntityFrameworkCore.Modeling;
using DMSpro.P42.MDM.Companies;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Identity;

namespace DMSpro.P42.MDM.EntityFrameworkCore;

public static class MDMDbContextModelCreatingExtensions
{
    public static void ConfigureMDM(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(MDMDbProperties.DbTablePrefix + "Questions", MDMDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */

        builder.Entity<Company>(b =>
    {
        b.ToTable(MDMDbProperties.DbTablePrefix + "Companies", MDMDbProperties.DbSchema);
        b.ConfigureByConvention();
        b.Property(x => x.TenantId).HasColumnName(nameof(Company.TenantId));
        b.Property(x => x.Code).HasColumnName(nameof(Company.Code)).IsRequired().HasMaxLength(CompanyConsts.CodeMaxLength);
        b.Property(x => x.Name).HasColumnName(nameof(Company.Name)).IsRequired().HasMaxLength(CompanyConsts.NameMaxLength);
        b.Property(x => x.Address1).HasColumnName(nameof(Company.Address1));
        b.HasMany(x => x.IdentityUsers).WithOne().HasForeignKey(x => x.CompanyId).IsRequired().OnDelete(DeleteBehavior.NoAction);
    });

        builder.Entity<CompanyIdentityUser>(b =>
{
    b.ToTable(MDMDbProperties.DbTablePrefix + "CompanyIdentityUser" + MDMDbProperties.DbSchema);
    b.ConfigureByConvention();

    b.HasKey(
    x => new { x.CompanyId, x.IdentityUserId }
    );

    b.HasOne<Company>().WithMany(x => x.IdentityUsers).HasForeignKey(x => x.CompanyId).IsRequired().OnDelete(DeleteBehavior.NoAction);
    b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.IdentityUserId).IsRequired().OnDelete(DeleteBehavior.NoAction);

    b.HasIndex(
        x => new { x.CompanyId, x.IdentityUserId }
    );
});
    }
}