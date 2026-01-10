using Rise.Domain.Dashboard;
using Rise.Shared.Shortcuts;

namespace Rise.Domain.Tests.Dashboard
{
    public class ShortcutShould
    {
        private Shortcut CreateShortcut(
            string title = "Title",
            ShortcutType shortcutType = ShortcutType.StudentLifeAndWellbeing,
            string icon = "icon",
            string label = "label",
            string linkUrl = "https://hogent.be",
            bool defaultForGuest = false
        ) =>
            new Shortcut(title, shortcutType, icon, label, linkUrl, defaultForGuest);

        [Fact]
        public void Can_Create_Shortcut_With_Valid_Constructor()
        {
            var shortcut = CreateShortcut(
                title: "My Schedule",
                shortcutType: ShortcutType.SchedulesAndCalendars,
                icon: "calendar-icon",
                label: "Schedule",
                linkUrl: "https://hogent.be/schedule",
                defaultForGuest: true
            );

            shortcut.Title.ShouldBe("My Schedule");
            shortcut.ShortcutType.ShouldBe(ShortcutType.SchedulesAndCalendars);
            shortcut.Icon.ShouldBe("calendar-icon");
            shortcut.Label.ShouldBe("Schedule");
            shortcut.LinkUrl.ShouldBe("https://hogent.be/schedule");
            shortcut.DefaultForGuest.ShouldBeTrue();
        }

        [Fact]
        public void Setting_Title_To_Null_Or_Whitespace_Should_Throw()
        {
            var shortcut = CreateShortcut();

            Should.Throw<ArgumentException>(() => shortcut.Title = null!);
            Should.Throw<ArgumentException>(() => shortcut.Title = "");
            Should.Throw<ArgumentException>(() => shortcut.Title = "   ");
        }

        [Fact]
        public void Setting_LinkUrl_To_Null_Or_Whitespace_Should_Throw()
        {
            var shortcut = CreateShortcut();

            Should.Throw<ArgumentException>(() => shortcut.LinkUrl = null!);
            Should.Throw<ArgumentException>(() => shortcut.LinkUrl = "");
            Should.Throw<ArgumentException>(() => shortcut.LinkUrl = "   ");
        }

        [Fact]
        public void Can_Set_Optional_Properties()
        {
            var shortcut = CreateShortcut();

            shortcut.Icon = "new-icon";
            shortcut.Label = "New Label";

            shortcut.Icon.ShouldBe("new-icon");
            shortcut.Label.ShouldBe("New Label");
        }
    }
}
