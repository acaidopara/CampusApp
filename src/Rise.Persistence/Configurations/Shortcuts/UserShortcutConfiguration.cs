using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Dashboard;

namespace Rise.Persistence.Configurations.Shortcuts;

public class UserShortcutConfiguration : EntityConfiguration<UserShortcut>
{
    public override void Configure(EntityTypeBuilder<UserShortcut> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserShortcuts");
        
        builder.HasKey(x => new { x.UserId, x.ShortcutId });

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserShortcuts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Shortcut)
            .WithMany(x => x.UserShortcuts)
            .HasForeignKey(x => x.ShortcutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Colour)
               .IsRequired()
               .HasDefaultValue("var(--secondary-color)");
    }
}