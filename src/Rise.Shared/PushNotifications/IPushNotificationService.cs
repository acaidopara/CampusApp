using Rise.Shared.Notifications;

namespace Rise.Shared.PushNotifications;

public interface IPushNotificationService
{
    Task AddAsync(NotificationSubscription sub);
    Task SendNotification(NotificationSubscription sub, string message);
    Task SendAll(string message);
}