using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Configurations.Users;

/// <summary>
/// Entity Framework Core configuration for the Student entity.
/// This class defines the database schema mappings, constraints, and relationships for Students.
/// Students inherit from User and are associated with courses and deadlines in the education domain.
/// </summary>
internal class StudentConfiguration : EntityConfiguration<Student>
{
    /// <summary>
    /// Configures the Student entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Student.</param>
    public override void Configure(EntityTypeBuilder<Student> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Students" table in the database
        builder.ToTable("Students");

        // Configure the StudentNumber property: required with a maximum length
        builder.Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(50); // Adjust max length as needed based on domain requirements

        // Configure the many-to-many relationship with Courses
        // Uses an implicit junction table "StudentCourses" for the association
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(sd => sd.StudentId);

        // Configure the one-to-many relationship with StudentDeadlines (junction)
        // A Student has many StudentDeadlines, each linking to a Deadline
        builder.HasMany(s => s.StudentDeadlines)
            .WithOne(sd => sd.Student)
            .HasForeignKey(sd => sd.StudentId);
    }
}