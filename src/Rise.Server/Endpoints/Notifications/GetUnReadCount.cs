using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications
{
    public class GetUnReadCount(INotificationService notificationService)
        : Endpoint<NotificationRequest.GetForUser, Result<NotificationResponse.UnreadCount>>
    {
        public override void Configure()
        {
            Get("/api/users/{userId}/notifications/unread");
            Roles(AppRoles.Student);
        }

        public override Task<Result<NotificationResponse.UnreadCount>> ExecuteAsync(NotificationRequest.GetForUser req, CancellationToken ct)
        {
            return notificationService.GetUserUnreadCountAsync(req, ct);
        }
    }
}
