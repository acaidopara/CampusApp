using Rise.Shared.PushNotifications;

namespace Rise.Server.Endpoints.Notifications;

public class SendTestNotification(
    IPushNotificationService pushService
) : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Post("/api/notifications/test");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(CancellationToken ct)
    {
        var message = @"{
            ""title"": ""Test Notification"",
            ""message"": ""This is a test push notification!""
        }";

        await pushService.SendAll(message);
        return Result.Success();
    }
}
