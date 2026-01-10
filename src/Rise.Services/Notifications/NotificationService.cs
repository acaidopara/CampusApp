using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Notifications;

namespace Rise.Services.Notifications;

public class NotificationService(ApplicationDbContext dbContext) : INotificationService
{
    public async Task<Result<NotificationResponse.Detail>> GetNotificationByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        var notification = await dbContext.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == request.Id, ctx);

        if (notification is null)
            return Result<NotificationResponse.Detail>.NotFound($"Notification with ID: {request.Id} not found.");

        var dto = new NotificationDto.Detail
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message ?? string.Empty,
            CreatedAt = notification.CreatedAt,
            LinkUrl = notification.LinkUrl,
            Subject = notification.Subject
        };

        return Result.Success(new NotificationResponse.Detail{ Notification = dto });
    }

    public async Task<Result<NotificationResponse.Index>> GetNotificationsAsync(QueryRequest.SkipTake request ,CancellationToken ctx = default)
    {
        var query = dbContext.Notifications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Title.Contains(request.SearchTerm));
        }

        var items = await query
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ctx);

        var dtos = items.Select(n => new NotificationDto.Index
        {
            Id = n.Id,
            Title = n.Title,
            CreatedAt = n.CreatedAt,
            IsRead = false,
            Subject = n.Subject
        });

        return Result.Success(new NotificationResponse.Index { Notifications = dtos });
    }

    public async Task<Result<NotificationResponse.UnreadCount>> GetUserUnreadCountAsync(NotificationRequest.GetForUser request, CancellationToken ctx = default)
    {
        if (request.UserId <= 0)
            return Result.Invalid(new ValidationError("UserId is required."));

        var userExists = await dbContext.Students 
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.UserId, ctx);

        if (!userExists)
            return Result.NotFound("User not found.");

        var count = await dbContext.UserNotifications
            .Where(un => un.UserId == request.UserId && !un.IsRead && !un.IsDeleted)
            .CountAsync(ctx);

        return Result.Success(new NotificationResponse.UnreadCount{ Count = count });
    }

    public async Task<Result<NotificationResponse.Index>> GetUserNotificationsAsync(NotificationRequest.GetForUser request, CancellationToken ct)
    {
        var query = dbContext.UserNotifications
            .Include(un => un.Notification)
            .Where(un => un.UserId == request.UserId && !un.IsDeleted)
            .AsQueryable();

        var topic = request.TopicTerm;
        var term = request.SearchTerm;

        query = query
        .Where(un =>
            string.IsNullOrWhiteSpace(topic) ||
            topic.Equals("Alles", StringComparison.OrdinalIgnoreCase) ||
            (topic.Equals("Gelezen", StringComparison.OrdinalIgnoreCase) && un.IsRead) ||
            (topic.Equals("Ongelezen", StringComparison.OrdinalIgnoreCase) && !un.IsRead))
        .Where(un =>
            string.IsNullOrWhiteSpace(term) ||
            (un.Notification != null && ( un.Notification.Title.Contains(term) || (un.Notification.Message != null && un.Notification.Message.Contains(term)))));

        var totalCount = await query.CountAsync(ct);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e.Notification!, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e.Notification!, request.OrderBy));
        }
        else
        {
            query = query.OrderByDescending(un => un.Notification != null ? un.Notification.CreatedAt : DateTime.MinValue);
        }

        var notifications = await query
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(un => new NotificationDto.Index
            {
                Id = un.Notification!.Id,
                Title = un.Notification.Title,
                CreatedAt = un.Notification.CreatedAt,
                IsRead = un.IsRead,
                Subject = un.Notification.Subject
            })
            .ToListAsync(ct);

        return Result.Success(new NotificationResponse.Index
        {
            Notifications = notifications,
            TotalCount = totalCount
        });
    }

    public async Task<Result> AddNotificationToUserAsync(NotificationRequest.AddToUser request, CancellationToken ct)
    {
        var notificationExists = await dbContext.Notifications
            .AnyAsync(n => n.Id == request.NotificationId, ct);

        if (!notificationExists)
            return Result.NotFound($"Notification with ID {request.NotificationId} not found.");

        var existing = await dbContext.UserNotifications
            .FirstOrDefaultAsync(un => un.UserId == request.UserId && un.NotificationId == request.NotificationId, ct);

        if (existing is not null && !existing.IsDeleted)
            return Result.Invalid(new ValidationError("Notification is already added for this user."));

        var userNotification = new Domain.Notifications.UserNotification(request.UserId, request.NotificationId);
        dbContext.UserNotifications.Add(userNotification);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> UpdateUserNotificationIsReadAsync(NotificationRequest.ChangeRead request, CancellationToken ct)
    {
        var userNotification = await dbContext.UserNotifications
            .FirstOrDefaultAsync(un => un.UserId == request.UserId && un.NotificationId == request.NotificationId, ct);

        if (userNotification is null)
            return Result.NotFound("User notification not found.");

        if (userNotification.IsRead == false)
            userNotification.MarkAsRead();

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteUserNotificationAsync(NotificationRequest.RemoveFromUser request, CancellationToken ct)
    {
        var userNotification = await dbContext.UserNotifications
        .FirstOrDefaultAsync(un => un.UserId == request.UserId
                                && un.NotificationId == request.NotificationId, ct);

        if (userNotification is null)
            return Result.NotFound("User notification not found.");

        userNotification.IsDeleted = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<NotificationResponse.Detail>> GetLastWarningNotificationAsync(NotificationRequest.GetForUser request, CancellationToken ct)
    {
        if (request.UserId <= 0)
            return Result.Invalid(new ValidationError("UserId is required."));


        var userExists = await dbContext.Students 
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.UserId, ct);

        if (!userExists)
            return Result.NotFound("User not found.");

        var lastWarning = await dbContext.UserNotifications
            .Include(un => un.Notification)
            .Where(un => un.UserId == request.UserId && !un.IsDeleted && un.Notification != null && un.Notification.Subject == "Warning")
            .AsNoTracking()
            .OrderByDescending(un => un.Notification!.CreatedAt)
            .Select(un => new NotificationDto.Detail
            {
                Id = un.Notification!.Id,
                Title = un.Notification.Title,
                Message = un.Notification.Message ?? string.Empty,
                CreatedAt = un.Notification.CreatedAt,
                LinkUrl = un.Notification.LinkUrl,
                Subject = un.Notification.Subject
            })
            .FirstOrDefaultAsync(ct);

        if (lastWarning is null)
            return Result.NotFound("Warning notification not found.");

        return Result.Success(new NotificationResponse.Detail
        {
            Notification = lastWarning
        });
    }
}