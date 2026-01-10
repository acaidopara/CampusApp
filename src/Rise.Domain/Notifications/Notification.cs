namespace Rise.Domain.Notifications
{
    public class Notification(string title, string message, string linkUrl, string subject) : Entity
    {
        public string Title { get; private set; } = Guard.Against.NullOrWhiteSpace(title, nameof(Title));
        public string? Message { get; private set; } = message;
        public string? LinkUrl { get; private set; } = linkUrl;
        public string Subject { get; private set; } = subject;
        public ICollection<UserNotification> UserNotifications { get; init; } = [];
    }
}
