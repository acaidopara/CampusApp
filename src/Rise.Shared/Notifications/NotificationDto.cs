
namespace Rise.Shared.Notifications
{
    public static class NotificationDto
    {
        public class Index
        {
            public int Id { get; set; }
            public required string Title { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRead { get; set; } = false;
            public required string Subject { get; set; }
        }

        public class Detail
        {
            public int Id { get; set; }
            public required string Title { get; set; }
            public required string Message { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? LinkUrl { get; set; }
            public required string Subject { get; set; }
        }
    }
}
