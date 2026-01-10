using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

public class GetLastWarningNotification(INotificationService notificationService)
    : Endpoint<NotificationRequest.GetForUser, Result<NotificationResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/users/{userId}/notifications/warning");
        Roles(AppRoles.Student);
    }

    public override Task<Result<NotificationResponse.Detail>> ExecuteAsync(NotificationRequest.GetForUser req, CancellationToken ct)
    {
        return notificationService.GetLastWarningNotificationAsync(req, ct);
    }
}
