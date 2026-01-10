using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications
{
    public class AddToUser(INotificationService notificationService)
    : Endpoint<NotificationRequest.AddToUser, Result>
    {
        public override void Configure()
        {
            Post("/api/users/{userId}/notifications/{notificationId}");
            Roles(AppRoles.Administrator);
        }

        public override async Task<Result> ExecuteAsync(NotificationRequest.AddToUser req, CancellationToken ct)
        {
            return await notificationService.AddNotificationToUserAsync(req, ct);
        }
    }
}
