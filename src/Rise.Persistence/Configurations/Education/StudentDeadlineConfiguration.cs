using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Education;

namespace Rise.Persistence.Configurations.Education;

/// <summary>
/// Entity Framework Core configuration for the StudentDeadline junction entity.
/// This class defines the database schema mappings, constraints, and relationships for StudentDeadlines.
/// StudentDeadline acts as a many-to-many junction between Students and Deadlines, tracking per-student completion.
/// </summary>
internal class StudentDeadlineConfiguration : EntityConfiguration<StudentDeadline>
{
    /// <summary>
    /// Configures the StudentDeadline entity type.
    /// Sets the table name, composite key, property constraints, and relationships.
    /// </summary>
    /// <param name="builder">The entity type builder for StudentDeadline.</param>
    public override void Configure(EntityTypeBuilder<StudentDeadline> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "StudentDeadlines" table in the database
        builder.ToTable("StudentDeadlines");

        // Configure the composite primary key (StudentId + DeadlineId)
        builder.HasKey(sd => new { sd.StudentId, sd.DeadlineId });

        // Configure the IsCompleted property: required boolean with default false
        builder.Property(sd => sd.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure the CompletionDate property: optional datetime
        builder.Property(sd => sd.CompletionDate)
            .HasColumnType("datetime");

        // Configure the many-to-one relationship to Student
        // A StudentDeadline belongs to one Student, and a Student has many StudentDeadlines
        builder.HasOne(sd => sd.Student)
            .WithMany(s => s.StudentDeadlines)
            .HasForeignKey(sd => sd.StudentId)
            .OnDelete(DeleteBehavior.Cascade); // Delete junction records if Student is deleted

        // Configure the many-to-one relationship to Deadline
        // A StudentDeadline belongs to one Deadline, and a Deadline has many StudentDeadlines
        builder.HasOne(sd => sd.Deadline)
            .WithMany(d => d.StudentDeadlines)
            .HasForeignKey(sd => sd.DeadlineId)
            .OnDelete(DeleteBehavior.Cascade); // Delete junction records if Deadline is deleted
    }
}