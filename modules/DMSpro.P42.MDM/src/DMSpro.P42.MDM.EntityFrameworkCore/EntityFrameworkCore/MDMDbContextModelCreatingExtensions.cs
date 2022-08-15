﻿using Microsoft.EntityFrameworkCore;
using Volo.Abp;

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
    }
}
