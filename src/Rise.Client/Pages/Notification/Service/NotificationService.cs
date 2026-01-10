using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Notifications;
using Rise.Client.Api;

namespace Rise.Client.Pages.Notification.Service
{
    public class NotificationService(TransportProvider transport) : INotificationService
    {
        public async Task<Result<NotificationResponse.Index>> GetNotificationsAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
            return (await transport.Current.GetAsync<NotificationResponse.Index>($"/api/notifications?{request.AsQuery()}", cancellationToken: ctx))!;
        }

        public async Task<Result<NotificationResponse.Detail>> GetNotificationByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default)
        {
            return (await transport.Current.GetAsync<NotificationResponse.Detail>($"/api/notifications/{request.Id}", cancellationToken: ctx))!;
        }

        public async Task<Result<NotificationResponse.UnreadCount>> GetUserUnreadCountAsync(NotificationRequest.GetForUser request, CancellationToken ctx = default)
        {
            return (await transport.Current.GetAsync<NotificationResponse.UnreadCount>($"/api/users/{request.UserId}/notifications/unread", cancellationToken: ctx))!;
        }

        public async Task<Result<NotificationResponse.Index>> GetUserNotificationsAsync(NotificationRequest.GetForUser request, CancellationToken ctx = default)
        {
            return (await transport.Current.GetAsync<NotificationResponse.Index>($"/api/users/{request.UserId}/notifications/filter/{request.TopicTerm}/?{request.AsQuery()}", cancellationToken: ctx))!;
        }

        public async Task<Result> AddNotificationToUserAsync(NotificationRequest.AddToUser request, CancellationToken ct = default)
        {
           return (await transport.Current.PostAsync<Result,NotificationRequest.AddToUser>($"/api/users/{request.UserId}/notifications/{request.NotificationId}", request, ct))!;
        }

        public async Task<Result> UpdateUserNotificationIsReadAsync(NotificationRequest.ChangeRead request, CancellationToken ct = default)
        {
            return (await transport.Current.PutAsync<NotificationRequest.ChangeRead >($"/api/users/{request.UserId}/notifications/{request.NotificationId}/read", request, ct,true))!;
        }

        public async Task<Result> DeleteUserNotificationAsync(NotificationRequest.RemoveFromUser request, CancellationToken ct)
        {
            return (await transport.Current.DelAsync($"/api/users/{request.UserId}/notifications/{request.NotificationId}", ct))!;
        }

        public async Task<Result<NotificationResponse.Detail>> GetLastWarningNotificationAsync(NotificationRequest.GetForUser request, CancellationToken ct)
        {
            return (await transport.Current.GetAsync<NotificationResponse.Detail>($"/api/users/{request.UserId}/notifications/warning", ct))!;
        }
    }
}