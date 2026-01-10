using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Departments;

namespace Rise.Persistence.Configurations.Departments;

internal class DepartmentConfiguration : EntityConfiguration<Department>
{
    public override void Configure(EntityTypeBuilder<Department> builder)
    {
        base.Configure(builder);

        builder.HasMany(d => d.Members)
            .WithOne(e => e.Department)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.Employees);
        builder.Ignore(x => x.Students);
    }
}