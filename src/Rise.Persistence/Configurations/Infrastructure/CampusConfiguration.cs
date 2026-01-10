using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Infrastructure;

namespace Rise.Persistence.Configurations.Infrastructure;

/// <summary>
/// Entity Framework Core configuration for the Campus entity.
/// This class defines the database schema mappings, constraints, and relationships for Campuses.
/// Campuses are top-level locations containing buildings, events, and emergencies.
/// </summary>
internal class CampusConfiguration : EntityConfiguration<Campus>
{
    /// <summary>
    /// Configures the Campus entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Campus.</param>
    public override void Configure(EntityTypeBuilder<Campus> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Campuses" table in the database
        builder.ToTable("Campuses");

        // Configure the Name property: required with a maximum length
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure the Description property: optional with a maximum length
        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        // Configure the ImageUrl property: optional with a maximum length
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);
        
        // Configure the TourUrl property: optional with a maximum length
        builder.Property(c => c.TourUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        
        // Configure the MapsUrl property: optional with a maximum length
        builder.Property(c => c.MapsUrl)
            .HasMaxLength(500);
        
        // Configure the Facilities property
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

        // Configure the Address as an owned entity
        builder.OwnsOne(c => c.Address, ab =>
        {
            ab.Property(a => a.Addressline1).HasColumnName("Street").IsRequired().HasMaxLength(200);
            ab.Property(a => a.Addressline2).HasColumnName("Number").HasMaxLength(50);
            ab.Property(a => a.PostalCode).HasColumnName("ZipCode").IsRequired().HasMaxLength(10);
            ab.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
        });

        // Configure the one-to-many relationship with Buildings
        builder.HasMany(c => c.Buildings)
            .WithOne(b => b.Campus)
            .HasForeignKey("CampusId");

        // Configure the one-to-many relationship with Events
        builder.HasMany(c => c.Events)
            .WithOne()
            .HasForeignKey("CampusId");

        // Configure the one-to-many relationship with Emergencies
        builder.HasMany(c => c.Emergencies)
            .WithOne()
            .HasForeignKey("CampusId");
    }
}