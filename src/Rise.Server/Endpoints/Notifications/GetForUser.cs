using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;


public class GetForUser(INotificationService notificationService) : Endpoint<NotificationRequest.GetForUser, Result<NotificationResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/users/{userId}/notifications/filter/{topicTerm}");
        Roles(AppRoles.Student);
    }

    public override Task<Result<NotificationResponse.Index>> ExecuteAsync(NotificationRequest.GetForUser req, CancellationToken ct)
    {
        return notificationService.GetUserNotificationsAsync(req, ct);
    }
}
