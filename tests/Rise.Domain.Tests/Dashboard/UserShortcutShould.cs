using Rise.Domain.Dashboard;

namespace Rise.Domain.Tests.Dashboard
{
    public class UserShortcutShould
    {
        [Fact]
        public void Can_Create_UserShortcut_With_Valid_Constructor()
        {
            var userShortcut = new UserShortcut(userId: 1, shortcutId: 5, position: 3);

            userShortcut.UserId.ShouldBe(1);
            userShortcut.ShortcutId.ShouldBe(5);
            userShortcut.Position.ShouldBe(3);
            userShortcut.User.ShouldBeNull();
            userShortcut.Shortcut.ShouldBeNull();
        }

        [Fact]
        public void Can_Update_Position()
        {
            var userShortcut = new UserShortcut(1, 2, 1);
            userShortcut.UpdatePosition(5);

            userShortcut.Position.ShouldBe(5);
        }
    }
}
