using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Entities;

namespace Rise.Persistence.Configurations.Entities;

/// <summary>
/// Specific configuration for <see cref="Product"/>.
/// </summary>
internal class MenuItemConfiguration : EntityConfiguration<MenuItem>
{
    public override void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Category).IsRequired();
        builder.Property(x => x.IsVeganAndHalal).HasDefaultValue(false).IsRequired();
        
    }
}