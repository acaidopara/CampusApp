namespace Rise.Shared.Notifications;

public static partial class UserNotificationDto
{
    public class Index
    {
        public int UserId { get; set; } = 0;
        public int NotificationId { get; set; } = 0;
        public bool IsRead { get; set; } = false;
    }
}
