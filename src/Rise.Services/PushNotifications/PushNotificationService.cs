using Microsoft.Extensions.Configuration;
using Rise.Shared.PushNotifications;
using WebPush;

namespace Rise.Services.PushNotifications;

public class PushNotificationService : IPushNotificationService
{
    private readonly IConfiguration _config;
    private readonly List<NotificationSubscription> _subscriptions = new();

    public PushNotificationService(IConfiguration config)
    {
        _config = config;
    }

    public Task AddAsync(NotificationSubscription sub)
    {
        if (!_subscriptions.Any(s => s.Endpoint == sub.Endpoint))
        {
            _subscriptions.Add(sub); 
        } 
        return Task.CompletedTask;
    }

    public async Task SendNotification(NotificationSubscription sub, string message)
    {
        var vapid = _config.GetSection("VapidSettings");
        var details = new VapidDetails(
            vapid["Subject"],
            vapid["PublicKey"],
            vapid["PrivateKey"]
        );

        var webPush = new WebPushClient();
        var pushSubscription = new PushSubscription(
            sub.Endpoint,
            sub.P256dh,
            sub.Auth
        );

        await webPush.SendNotificationAsync(
            pushSubscription,
            message,
            details
        );
    }

    public async Task SendAll(string message)
    {
        foreach (var sub in _subscriptions)
        {
            await SendNotification(sub, message);
        }
    }
}