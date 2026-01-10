using Rise.Shared.Common;

namespace Rise.Shared.Notifications
{
    public interface INotificationService
    {
        Task<Result<NotificationResponse.Index>> GetNotificationsAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
        Task<Result<NotificationResponse.Detail>> GetNotificationByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default);
        Task<Result<NotificationResponse.UnreadCount>> GetUserUnreadCountAsync(NotificationRequest.GetForUser request, CancellationToken ctx = default);
        Task<Result<NotificationResponse.Index>> GetUserNotificationsAsync(NotificationRequest.GetForUser request, CancellationToken ct = default);
        Task<Result> AddNotificationToUserAsync(NotificationRequest.AddToUser request, CancellationToken ct = default);
        Task<Result> UpdateUserNotificationIsReadAsync(NotificationRequest.ChangeRead request, CancellationToken ct = default);
        Task<Result> DeleteUserNotificationAsync(NotificationRequest.RemoveFromUser request, CancellationToken ct = default);
        Task<Result<NotificationResponse.Detail>> GetLastWarningNotificationAsync(NotificationRequest.GetForUser request, CancellationToken ct = default);
    }
}
