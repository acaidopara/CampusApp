using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Configurations.Users;

internal class TeacherConfiguration : EntityConfiguration<Teacher>
{
    public override void Configure(EntityTypeBuilder<Teacher> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Teachers");
        
        builder.HasMany(e => e.Lessons)
            .WithMany(l => l.Teachers)
            .UsingEntity(join => join.ToTable("TeacherLessons"));
        
        builder.Property(t => t.IsAbsent).IsRequired();
    }
}