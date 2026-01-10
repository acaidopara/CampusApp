using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

public class RemoveNotificationFromUser(INotificationService notificationService) : Endpoint<NotificationRequest.RemoveFromUser, Result>
{
    public override void Configure()
    {
        Delete("/api/users/{userId}/notifications/{notificationId}");
        Roles(AppRoles.Student);
    }

    public override Task<Result> ExecuteAsync(NotificationRequest.RemoveFromUser req, CancellationToken ct)
    {
        return notificationService.DeleteUserNotificationAsync(req, ct);
    }
}
