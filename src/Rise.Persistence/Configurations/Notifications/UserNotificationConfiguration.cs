using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;

namespace Rise.Persistence.Configurations.Notifications;

    public class UserNotificationConfiguration : EntityConfiguration<UserNotification>
    {
        public override void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserNotifications");

            builder.HasKey(x => new { x.UserId, x.NotificationId });
            
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserNotifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Notification)
                .WithMany(x => x.UserNotifications)
                .HasForeignKey(x => x.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
