namespace Rise.Shared.Shortcuts;

public static class ShortcutDto
{
    public class Index
    {
        public int Id { get; init; }
        public string? Title { get; init; } = string.Empty;
        public string? Icon { get; init; } = string.Empty;
        public string? Label { get; init; } = string.Empty;
        public string? LinkUrl { get; init; } = string.Empty;
        public ShortcutType ShortcutType { get; init; }
        public string Colour { get; set; } = "var(--secondary-color)";
    }
}