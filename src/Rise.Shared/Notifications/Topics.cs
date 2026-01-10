namespace Rise.Shared.Notifications;

public record NotificationTopic(string Name, string? Icon);

public static class NotificationTopics
{
    public static readonly NotificationTopic Default = new("Default", null);
    public static readonly NotificationTopic Warning = new("Warning", null);
    public static readonly NotificationTopic Aankondiging = new("Aankondiging", null);
    public static readonly NotificationTopic Deadline = new("Deadline", null);
    public static readonly NotificationTopic NewsOfEvent = new("News/Event", null);
    public static readonly NotificationTopic AfwezigheidLector = new("Afwezig", null);
    public static readonly NotificationTopic Leswijziging = new("Leswijziging", null);

    public static readonly List<NotificationTopic> AllTopics = new() { Default, Warning, Aankondiging, Deadline, NewsOfEvent, AfwezigheidLector, Leswijziging };
}