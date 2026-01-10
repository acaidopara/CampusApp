using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Notifications
{
    public class Index(INotificationService notificationService) : Endpoint<QueryRequest.SkipTake, Result<NotificationResponse.Index>>
    {
        public override void Configure()
        {
            Get("/api/notifications");
            Roles(AppRoles.Administrator);
        }

        public override Task<Result<NotificationResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
        {
            return notificationService.GetNotificationsAsync(req, ct);
        }
    }
}
