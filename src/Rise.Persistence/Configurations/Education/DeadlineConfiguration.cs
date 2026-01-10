using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Education;

namespace Rise.Persistence.Configurations.Education;

/// <summary>
/// Entity Framework Core configuration for the Deadline entity.
/// This class defines the database schema mappings, constraints, and relationships for Deadlines.
/// Deadlines are time-bound tasks in the education domain, optionally linked to courses and assigned to students via junctions.
/// </summary>
internal class DeadlineConfiguration : EntityConfiguration<Deadline>
{
    /// <summary>
    /// Configures the Deadline entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Deadline.</param>
    public override void Configure(EntityTypeBuilder<Deadline> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Deadlines" table in the database
        builder.ToTable("Deadlines");

        // Configure the Title property: required with a maximum length
        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(100); // Adjust as needed based on domain requirements

        // Configure the Description property: optional with a maximum length
        builder.Property(d => d.Description)
            .HasMaxLength(500); // Optional, adjust for storage needs

        // Configure the DueDate property: required datetime
        builder.Property(d => d.DueDate)
            .IsRequired()
            .HasColumnType("datetime");

        // Configure the StartDate property: required datetime
        builder.Property(d => d.StartDate)
            .IsRequired()
            .HasColumnType("datetime");

        // The one-to-many relationship from Course is configured in CourseConfiguration

        // Configure the one-to-many relationship with StudentDeadlines (junction)
        // A Deadline can have many StudentDeadlines, each linking to a Student
        builder.HasMany(d => d.StudentDeadlines)
            .WithOne(sd => sd.Deadline)
            .HasForeignKey(sd => sd.DeadlineId);
    }
}