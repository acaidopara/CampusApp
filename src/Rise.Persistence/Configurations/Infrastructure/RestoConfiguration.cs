using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Infrastructure;

namespace Rise.Persistence.Configurations.Infrastructure;

internal class RestoConfiguration : EntityConfiguration<Resto>
{
    public override void Configure(EntityTypeBuilder<Resto> builder)
    {
        base.Configure(builder);

        builder.ToTable("Restos");

        // Configure required properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Coordinates)
            .IsRequired()
            .HasMaxLength(100);  // Adjust length as needed

        // Configure relationship to Building
        builder.HasOne(r => r.Building)
            .WithMany(b => b.Restos)
            .HasForeignKey("BuildingId")  // Shadow FK
            .IsRequired();

        builder.HasOne(r => r.Menu)
     .WithMany(m => m.Restos)
     .HasForeignKey("MenuId")
     .IsRequired(false);

    }
}