using Rise.Shared.Notifications;

namespace Rise.Domain.Tests.Notifications
{
    public class NotificationShould
    {
        [Fact]
        public void Should_Create_Notification_With_All_Required_Fields()
        {
            var notification = new Domain.Notifications.Notification(
                "System maintenance",
                "Planned maintenance tonight at 02:00.",
                "https://hogent.be/maintenance", NotificationTopics.Default.Name);

            notification.ShouldNotBeNull();
            notification.Title.ShouldBe("System maintenance");
            notification.Message.ShouldBe("Planned maintenance tonight at 02:00.");
            notification.LinkUrl.ShouldBe("https://hogent.be/maintenance");
            notification.Subject.ShouldBe(NotificationTopics.Default.Name);
        }

        [Fact]
        public void Should_Allow_Null_Message_And_LinkUrl()
        {
            var notification = new Domain.Notifications.Notification("Info only", null!, null!, NotificationTopics.Default.Name);

            notification.Title.ShouldBe("Info only");
            notification.Message.ShouldBeNull();
            notification.LinkUrl.ShouldBeNull();
            notification.Subject.ShouldBe(NotificationTopics.Default.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Creating_Notification_With_NullOrWhitespace_Title_Should_Throw_Exception(string? title)
        {
            Exception exception = Record.Exception(() => new Domain.Notifications.Notification(title!, "msg", "https://link", NotificationTopics.Default.Name));
            exception.ShouldNotBeNull();
        }
    }
}
