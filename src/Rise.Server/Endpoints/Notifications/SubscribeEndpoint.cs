
using Rise.Shared.Identity;
using Rise.Shared.PushNotifications;

namespace Rise.Server.Endpoints.Notifications
{
    public class SubscribeUser(
        IPushNotificationService pushService
    ) : Endpoint<NotificationSubscription, Result>
    {
        public override void Configure()
        {
            Post("/api/notifications/subscribe");
            Roles(AppRoles.Student);
        }

        public override async Task<Result> ExecuteAsync(NotificationSubscription req, CancellationToken ct)
        {
            await pushService.AddAsync(req);
            return Result.Success();
        }
    }
}
