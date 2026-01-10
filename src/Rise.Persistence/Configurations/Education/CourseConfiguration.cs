using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Education;

namespace Rise.Persistence.Configurations.Education;

internal class CourseConfiguration : EntityConfiguration<Course>
{
    public override void Configure(EntityTypeBuilder<Course> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Courses");
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(c => c.StudyField)
            .WithMany(sf => sf.Courses)
            .HasForeignKey("StudyFieldId")
            .IsRequired();

        builder.HasMany(c => c.Deadlines)
            .WithOne(d => d.Course)
            .HasForeignKey("CourseId")
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey("CourseId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}