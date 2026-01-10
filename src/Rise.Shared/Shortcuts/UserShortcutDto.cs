namespace Rise.Shared.Shortcuts;

public static partial class UserShortcutDto
{
    public class Index
    {
        public int UserId { get; set; } = 0;
        public int ShortcutId { get; set; } = 0;
        public int Position { get; set; } = 0;
        public string Colour { get; set; } = "var(--secondary-color)";
    }
}