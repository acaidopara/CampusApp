using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rise.Persistence.Configurations.Identity;

/// <summary>
/// Configuration for the Identity tables.
/// </summary>
internal class IdentityConfiguration :
    IEntityTypeConfiguration<IdentityUser>,
    IEntityTypeConfiguration<IdentityRole>,
    IEntityTypeConfiguration<IdentityUserRole<string>>,
    IEntityTypeConfiguration<IdentityUserClaim<string>>,
    IEntityTypeConfiguration<IdentityUserLogin<string>>,
    IEntityTypeConfiguration<IdentityRoleClaim<string>>,
    IEntityTypeConfiguration<IdentityUserToken<string>>
{
    // Configures the IdentityUser tables.
    // NOTE:
    // If you want to use a separate schema for your Identity tables,
    // you can specify the schema as "auth" like this:
    //     builder.ToTable("Users", "auth");
    // However, be aware that SQLite does NOT support schemas, so this only works with SQL Server, MarioDB, PostgreSQL,...
    // The default below will work on any provider.

    public void Configure(EntityTypeBuilder<IdentityUser> builder)
        => builder.ToTable("Account");

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.Property(r => r.Id).HasMaxLength(36);
        builder.Property(r => r.Name).HasMaxLength(256);
        builder.Property(r => r.NormalizedName).HasMaxLength(256);
        builder.Property(r => r.ConcurrencyStamp).HasMaxLength(36);
    }

    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
       {
        builder.Property(r => r.UserId).HasMaxLength(36);
        builder.Property(r => r.RoleId).HasMaxLength(36);
       
    }

    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        => builder.ToTable("AccountClaim");

    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        => builder.ToTable("AccountLogin");

    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        => builder.ToTable("RoleClaim");

    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        => builder.ToTable("AccountToken");
}