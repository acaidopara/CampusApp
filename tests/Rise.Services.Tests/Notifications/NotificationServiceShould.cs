using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Departments;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Notifications;
using Rise.Shared.Common;
using Rise.Shared.Notifications;

namespace Rise.Services.Tests.Notifications;

[Collection("IntegrationTests")]
public class NotificationServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required INotificationService NotificationService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
        NotificationService = CreateService(Db);
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private static NotificationService CreateService(ApplicationDbContext db)
    {
        return new NotificationService(db);
    }

    private static Domain.Users.Student CreateStudent(string accountId)
    {
        return new Domain.Users.Student()
        {
            Firstname = "Test",
            Lastname = "Test",
            AccountId = accountId,
            StudentNumber = "S123",
            PreferedCampus = "Main",
            Birthdate = new DateTime(2000, 1, 1),
            Department = new Department
            {
                Name = "IT en Digitale Innovatie",
                Description = "Opleidingen in toegepaste informatica, digitaal ontwerp en IT-beheer",
                DepartmentType = DepartmentType.Department
            },
            Email = new EmailAddress("test@student.hogent.be")
        };
    }
    private static Domain.Notifications.Notification CreateNotification(string title)
    {
        return new Domain.Notifications.Notification(title, "Test message", "https://wwww.test.com", "Default");
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnNotFound_WhenNotificationDoesNotExist()
    {
        var request = new GetByIdRequest.GetById { Id = 999 };

        var result = await NotificationService.GetNotificationByIdAsync(request);

        Assert.True(result.IsNotFound());
        Assert.Equal("Notification not found", result.Errors.First());
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnNotification_WhenExists()
    {
        var notification = CreateNotification("Test Notification");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var request = new GetByIdRequest.GetById { Id = notification.Id };

        var result = await NotificationService.GetNotificationByIdAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Test Notification", result.Value.Notification.Title);
        Assert.Equal("Test message", result.Value.Notification.Message);
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldReturnEmpty_WhenNoNotifications()
    {
        var request = new QueryRequest.SkipTake();

        var result = await NotificationService.GetNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Notifications);
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldReturnAllNotifications()
    {
        var n1 = CreateNotification("Notification 1");
        var n2 = CreateNotification("Notification 2");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake();

        var result = await NotificationService.GetNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Notifications.Count());
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldFilterBySearchTerm()
    {
        var n1 = CreateNotification("Exam Notification");
        var n2 = CreateNotification("Homework Notification");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake { SearchTerm = "Exam" };

        var result = await NotificationService.GetNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        Assert.Equal("Exam Notification", result.Value.Notifications.First().Title);
    }

    [Fact]
    public async Task GetUserUnreadCountAsync_ShouldReturnInvalid_WhenUserIdIsZero()
    {
        var request = new NotificationRequest.GetForUser { UserId = 0 };

        var result = await NotificationService.GetUserUnreadCountAsync(request);

        Assert.True(result.IsInvalid());
        Assert.Contains(result.ValidationErrors, e => e.ErrorMessage == "UserId is required.");
    }

    [Fact]
    public async Task GetUserUnreadCountAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var request = new NotificationRequest.GetForUser { UserId = 999 };

        var result = await NotificationService.GetUserUnreadCountAsync(request);

        Assert.True(result.IsNotFound());
        Assert.Equal("User not found.", result.Errors.First());
    }

    [Fact]
    public async Task GetUserUnreadCountAsync_ShouldReturnNotFound_WhenNoUnreadNotifications()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id };

        var result = await NotificationService.GetUserUnreadCountAsync(request);

        Assert.False(result.IsConflict());
    }

    [Fact]
    public async Task GetUserUnreadCountAsync_ShouldReturnCorrectCount_WhenUnreadNotificationsExist()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id };

        var result = await NotificationService.GetUserUnreadCountAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Count);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnEmpty_WhenNoNotifications()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Notifications);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnNotifications_WhenExist()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test Notification");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        Assert.Equal("Test Notification", result.Value.Notifications.First().Title);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldFilterBySearchTerm()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("Exam Notification");
        var n2 = CreateNotification("Homework Notification");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        Db.UserNotifications.AddRange(
            new Domain.Notifications.UserNotification(user.Id, n1.Id),
            new Domain.Notifications.UserNotification(user.Id, n2.Id)
        );
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id, SearchTerm = "Exam" };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        Assert.Equal("Exam Notification", result.Value.Notifications.First().Title);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldFilterByTopicTerm_Ongelezen()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("Unread Notification");
        var n2 = CreateNotification("Read Notification");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        var un1 = new Domain.Notifications.UserNotification(user.Id, n1.Id); 
        var un2 = new Domain.Notifications.UserNotification(user.Id, n2.Id);
        un2.MarkAsRead(); 
        Db.UserNotifications.AddRange(un1, un2);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id, TopicTerm = "Ongelezen" };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        Assert.Equal("Unread Notification", result.Value.Notifications.First().Title);
        Assert.False(result.Value.Notifications.First().IsRead);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldFilterByTopicTerm_Gelezen()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("Unread Notification");
        var n2 = CreateNotification("Read Notification");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        var un1 = new Domain.Notifications.UserNotification(user.Id, n1.Id);
        var un2 = new Domain.Notifications.UserNotification(user.Id, n2.Id);
        un2.MarkAsRead();
        Db.UserNotifications.AddRange(un1, un2);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id, TopicTerm = "Gelezen" };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        Assert.Equal("Read Notification", result.Value.Notifications.First().Title);
        Assert.True(result.Value.Notifications.First().IsRead);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnAll_WhenTopicTermIsAlles()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("Notification 1");
        var n2 = CreateNotification("Notification 2");
        Db.Notifications.AddRange(n1, n2);
        await Db.SaveChangesAsync();

        var un1 = new Domain.Notifications.UserNotification(user.Id, n1.Id);
        var un2 = new Domain.Notifications.UserNotification(user.Id, n2.Id);
        un2.MarkAsRead();
        Db.UserNotifications.AddRange(un1, un2);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id, TopicTerm = "Alles" };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Notifications.Count());
        Assert.Equal(2, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOrderByCreatedAtDescending_ByDefault()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("N1");
        n1.CreatedAt = DateTime.UtcNow.AddDays(-2);
        var n2 = CreateNotification("N2");
        n2.CreatedAt = DateTime.UtcNow.AddDays(-1);
        var n3 = CreateNotification("N3");
        n3.CreatedAt = DateTime.UtcNow;
        Db.Notifications.AddRange(n1, n2, n3);
        await Db.SaveChangesAsync();

        Db.UserNotifications.AddRange(
            new Domain.Notifications.UserNotification(user.Id, n1.Id),
            new Domain.Notifications.UserNotification(user.Id, n2.Id),
            new Domain.Notifications.UserNotification(user.Id, n3.Id)
        );
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser { UserId = user.Id };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        var notifications = result.Value.Notifications.ToList();
        Assert.Equal("N3", notifications[0].Title);
        Assert.Equal("N2", notifications[1].Title);
        Assert.Equal("N1", notifications[2].Title);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOrderByTitleAscending()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("C");
        var n2 = CreateNotification("A");
        var n3 = CreateNotification("B");
        Db.Notifications.AddRange(n1, n2, n3);
        await Db.SaveChangesAsync();

        Db.UserNotifications.AddRange(
            new Domain.Notifications.UserNotification(user.Id, n1.Id),
            new Domain.Notifications.UserNotification(user.Id, n2.Id),
            new Domain.Notifications.UserNotification(user.Id, n3.Id)
        );
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser
        {
            UserId = user.Id,
            OrderBy = "Title",
            OrderDescending = false
        };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        var notifications = result.Value.Notifications.ToList();
        Assert.Equal("A", notifications[0].Title);
        Assert.Equal("B", notifications[1].Title);
        Assert.Equal("C", notifications[2].Title);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOrderByTitleDescending()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var n1 = CreateNotification("C");
        var n2 = CreateNotification("A");
        var n3 = CreateNotification("B");
        Db.Notifications.AddRange(n1, n2, n3);
        await Db.SaveChangesAsync();

        Db.UserNotifications.AddRange(
            new Domain.Notifications.UserNotification(user.Id, n1.Id),
            new Domain.Notifications.UserNotification(user.Id, n2.Id),
            new Domain.Notifications.UserNotification(user.Id, n3.Id)
        );
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.GetForUser
        {
            UserId = user.Id,
            OrderBy = "Title",
            OrderDescending = true
        };

        var result = await NotificationService.GetUserNotificationsAsync(request);

        Assert.True(result.IsSuccess);
        var notifications = result.Value.Notifications.ToList();
        Assert.Equal("C", notifications[0].Title);
        Assert.Equal("B", notifications[1].Title);
        Assert.Equal("A", notifications[2].Title);
    }

    [Fact]
    public async Task AddNotificationToUserAsync_ShouldReturnNotFound_WhenNotificationDoesNotExist()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.AddToUser { UserId = user.Id, NotificationId = 999 };

        var result = await NotificationService.AddNotificationToUserAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task AddNotificationToUserAsync_ShouldReturnInvalid_WhenAlreadyAddedAndNotDeleted()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.AddToUser { UserId = user.Id, NotificationId = notification.Id };

        var result = await NotificationService.AddNotificationToUserAsync(request);

        Assert.True(result.IsInvalid());
        Assert.Contains(result.ValidationErrors, e => e.ErrorMessage == "Notification is already added for this user.");
    }

    [Fact]
    public async Task AddNotificationToUserAsync_ShouldAddSuccessfully()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.AddToUser { UserId = user.Id, NotificationId = notification.Id };

        var result = await NotificationService.AddNotificationToUserAsync(request);

        Assert.True(result.IsSuccess);

        var added = await Db.UserNotifications.FirstOrDefaultAsync(un => un.UserId == user.Id && un.NotificationId == notification.Id);
        Assert.NotNull(added);
        Assert.False(added.IsRead);
    }

    [Fact]
    public async Task UpdateUserNotificationIsReadAsync_ShouldReturnNotFound_WhenUserNotificationDoesNotExist()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.ChangeRead { UserId = user.Id, NotificationId = 999 };

        var result = await NotificationService.UpdateUserNotificationIsReadAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task UpdateUserNotificationIsReadAsync_ShouldMarkAsRead_WhenUnread()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.ChangeRead { UserId = user.Id, NotificationId = notification.Id };

        var result = await NotificationService.UpdateUserNotificationIsReadAsync(request);

        Assert.True(result.IsSuccess);

        await Db.Entry(userNotification).ReloadAsync();
        Assert.True(userNotification.IsRead);
    }

    [Fact]
    public async Task UpdateUserNotificationIsReadAsync_ShouldDoNothing_WhenAlreadyRead()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        userNotification.MarkAsRead();
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.ChangeRead { UserId = user.Id, NotificationId = notification.Id };

        var result = await NotificationService.UpdateUserNotificationIsReadAsync(request);

        Assert.True(result.IsSuccess);

        await Db.Entry(userNotification).ReloadAsync();
        Assert.True(userNotification.IsRead);
    }

    [Fact]
    public async Task DeleteUserNotificationAsync_ShouldReturnNotFound_WhenUserNotificationDoesNotExist()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.RemoveFromUser { UserId = user.Id, NotificationId = 999 };

        var result = await NotificationService.DeleteUserNotificationAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task DeleteUserNotificationAsync_ShouldDeleteSuccessfully()
    {
        var user = CreateStudent("abc");
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var notification = CreateNotification("Test");
        Db.Notifications.Add(notification);
        await Db.SaveChangesAsync();

        var userNotification = new Domain.Notifications.UserNotification(user.Id, notification.Id);
        Db.UserNotifications.Add(userNotification);
        await Db.SaveChangesAsync();

        var request = new NotificationRequest.RemoveFromUser { UserId = user.Id, NotificationId = notification.Id };

        var result = await NotificationService.DeleteUserNotificationAsync(request);

        Assert.True(result.IsSuccess);

        var deleted = await Db.UserNotifications.FirstOrDefaultAsync(un => un.UserId == user.Id && un.NotificationId == notification.Id);
        Assert.Null(deleted);
    }
}