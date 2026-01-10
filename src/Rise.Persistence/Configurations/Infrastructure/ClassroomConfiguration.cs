using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Infrastructure;

namespace Rise.Persistence.Configurations.Infrastructure;

/// <summary>
/// Entity Framework Core configuration for the Classroom entity.
/// This class defines the database schema mappings, constraints, and relationships for Classrooms.
/// Classrooms are spaces within buildings used for lessons.
/// </summary>
internal class ClassroomConfiguration : EntityConfiguration<Classroom>
{
    /// <summary>
    /// Configures the Classroom entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Classroom.</param>
    public override void Configure(EntityTypeBuilder<Classroom> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Classrooms" table in the database
        builder.ToTable("Classrooms");

        // Configure the Number property: required with a maximum length
        builder.Property(c => c.Number)
            .IsRequired()
            .HasMaxLength(50);

        // Configure the Name property: required with a maximum length
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure the Description property: optional with a maximum length
        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // Configure the Category property: required with a maximum length
        builder.Property(c => c.Category)
            .IsRequired()
            .HasMaxLength(50);

        // Configure the Floor property: required with a maximum length
        builder.Property(c => c.Floor)
            .IsRequired()
            .HasMaxLength(50);

        // Configure the relationship with Building (many-to-one)
        builder.HasOne(c => c.Building)
            .WithMany(b => b.Classrooms)
            .HasForeignKey("BuildingId")
            .IsRequired();

        // Configure the many-to-many relationship with Lessons (assuming direct many-to-many)
        builder.HasMany(c => c.Lessons)
            .WithMany(l => l.Classrooms); // Adjust if Lesson has a Classrooms property
    }
}