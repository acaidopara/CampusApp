using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Education;

namespace Rise.Persistence.Configurations.Education;

internal class LessonConfiguration : EntityConfiguration<Lesson>
{
    public override void Configure(EntityTypeBuilder<Lesson> builder)
    {
        base.Configure(builder);

        builder.HasMany(l => l.Classrooms)
            .WithMany(c => c.Lessons)
            .UsingEntity(j => j.ToTable("LessonClassrooms"));

        builder
            .HasMany(l => l.Teachers)
            .WithMany(t => t.Lessons)
            .UsingEntity(j => j.ToTable("LessonTeachers"));

        builder
            .HasMany(l => l.ClassGroups)
            .WithMany(s => s.Lessons)
            .UsingEntity(j => j.ToTable("LessonStudents"));
    }
}