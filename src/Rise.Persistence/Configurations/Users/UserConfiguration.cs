using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Configurations.Users;

/// <summary>
/// Specific configuration for <see cref="User"/>.
/// </summary>
internal class UserConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");
        
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(x => x.Value).IsRequired().HasMaxLength(50).HasColumnName(nameof(User.Email));
        }).Navigation(x => x.Email).IsRequired();
    }
}
