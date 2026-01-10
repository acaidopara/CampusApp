using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Notifications
{
    public class ChangeRead(INotificationService notificationService) : Endpoint<NotificationRequest.ChangeRead, Result>
    {
        public override void Configure()
        {
            Put("/api/users/{userId}/notifications/{notificationId}/read");
            Roles(AppRoles.Student);
        }

        public override async Task<Result> ExecuteAsync(NotificationRequest.ChangeRead req, CancellationToken ct)
        {
            return await notificationService.UpdateUserNotificationIsReadAsync(req, ct);
        }
    }
}
