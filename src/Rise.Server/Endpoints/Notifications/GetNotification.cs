using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications
{
    public class GetNotification(INotificationService notificationService)
        : Endpoint<GetByIdRequest.GetById, Result<NotificationResponse.Detail>>
    {
        public override void Configure()
        {
            Get("/api/notifications/{id}");
            Roles(AppRoles.Student);
        }

        public override Task<Result<NotificationResponse.Detail>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
        {
            return notificationService.GetNotificationByIdAsync(req, ct);
        }
    }
}
