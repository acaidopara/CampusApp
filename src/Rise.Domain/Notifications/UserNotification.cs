using Rise.Domain.Users;

namespace Rise.Domain.Notifications
{
    public class UserNotification : Entity
    {
        public int UserId { get; private set; }
        public int NotificationId { get; private set; }
        public bool IsRead { get; private set; }
        public User? User { get; private set; }
        public Notification? Notification { get; private set; }

        protected UserNotification() { }

        public UserNotification(int userId, int notificationId, bool isRead = false)
        {
            UserId = userId;
            NotificationId = notificationId;
            IsRead = isRead;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
