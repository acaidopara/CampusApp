using Rise.Domain.Dashboard;
using Rise.Domain.Departments;
using Rise.Domain.Notifications;

namespace Rise.Domain.Users;

public abstract class User : Entity
{
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    
    public required string AccountId { get; init; }
    public required Department Department { get; set; }
    public required EmailAddress Email { get; init; }
    
    public required DateTime Birthdate { get; init; }
    
    public ICollection<UserShortcut> UserShortcuts { get; set; } = [];

    public ICollection<UserNotification> UserNotifications { get; set; } = [];

}