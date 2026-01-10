using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;

namespace Rise.Persistence.Configurations.Notifications
{
    public class NotificationConfiguration : EntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);
            builder.ToTable("Notifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title)
                   .HasMaxLength(200)
                   .IsRequired();
            builder.Property(x => x.Message)
                   .HasColumnType("LONGTEXT");
            builder.Property(x => x.LinkUrl)
                   .HasMaxLength(256);
            builder.Property(x => x.Subject)
               .IsRequired()
               .HasMaxLength(50);
        }
    }
}
