namespace Rise.Shared.PushNotifications;

public class NotificationSubscription
{
    public string? Endpoint { get; set; }
    public string? P256dh { get; set; }
    public string? Auth { get; set; }
}