using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Entities;

namespace Rise.Persistence.Configurations.Entities;

/// <summary>
/// Specific configuration for <see cref="Product"/>.
/// </summary>
internal class MenuConfiguration : EntityConfiguration<Menu>
{
    public override void Configure(EntityTypeBuilder<Menu> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StartDate).IsRequired().HasDefaultValue(DateTime.Now);
        builder.Property(x => x.DescriptionMenu).IsRequired(false).HasColumnType("text") ;
        builder.Property(m => m.ItemsJson)
        .HasColumnType("json")
        .IsRequired()
        .HasDefaultValueSql("'{}'");


    }
}