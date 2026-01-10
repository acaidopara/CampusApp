namespace Rise.Shared.Notifications
{
    public static partial class NotificationResponse
    {
        public class Index
        {
            public IEnumerable<NotificationDto.Index> Notifications { get; set; } = [];
            public int TotalCount { get; set; }
        }
        public class Detail
        {
            public NotificationDto.Detail Notification { get; set; } = null!;
        }
        public class UnreadCount
        {
            public int Count { get; set; }
        }
    }
}
