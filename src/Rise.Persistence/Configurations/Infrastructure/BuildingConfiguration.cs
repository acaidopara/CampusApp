using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Infrastructure;

namespace Rise.Persistence.Configurations.Infrastructure;

/// <summary>
/// Entity Framework Core configuration for the Building entity.
/// This class defines the database schema mappings, constraints, and relationships for Buildings.
/// Buildings represent physical structures on a campus, containing classrooms and restos.
/// </summary>
internal class BuildingConfiguration : EntityConfiguration<Building>
{
    /// <summary>
    /// Configures the Building entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Building.</param>
    public override void Configure(EntityTypeBuilder<Building> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Buildings" table in the database
        builder.ToTable("Buildings");

        // Configure the Name property: required with a maximum length
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure the Description property: optional with a maximum length
        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        // Configure the ImageUrl property: optional with a maximum length
        builder.Property(b => b.ImageUrl)
            .HasMaxLength(500);

        // Configure the Address as an owned entity
        builder.OwnsOne(b => b.Address, ab =>
        {
            ab.Property(a => a.Addressline1).HasColumnName("Street").IsRequired().HasMaxLength(200);
            ab.Property(a => a.Addressline2).HasColumnName("Number").HasMaxLength(50);
            ab.Property(a => a.PostalCode).HasColumnName("ZipCode").IsRequired().HasMaxLength(10);
            ab.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
        });
        
        builder.OwnsOne(c => c.Facilities, fb =>
        {
            fb.Property(f => f.Library);
            fb.Property(f => f.RitaHelpdesk);
            fb.Property(f => f.RevolteRoom);
            fb.Property(f => f.ParkingLot);
            fb.Property(f => f.BikeStorage);
            fb.Property(f => f.StudentShop);
            fb.Property(f => f.Restaurant);
            fb.Property(f => f.Cafeteria);
            fb.Property(f => f.SportsHall);
            fb.Property(f => f.Stuvo);
            fb.Property(f => f.Lockers);
        });

        // Configure the relationship with Campus (many-to-one)
        builder.HasOne(b => b.Campus)
            .WithMany(c => c.Buildings)
            .HasForeignKey("CampusId")
            .IsRequired();

        builder.HasMany(b => b.Classrooms)
            .WithOne(c => c.Building)
            .HasForeignKey("BuildingId");  

        builder.HasMany(b => b.Restos)
            .WithOne(r => r.Building)
            .HasForeignKey("BuildingId"); 
    }
}