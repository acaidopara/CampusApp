using Rise.Domain.Users;

namespace Rise.Domain.Dashboard;

public class UserShortcut : Entity
{
    public int UserId { get; private set; }
    public int ShortcutId { get; private set; }
    
    public int Position { get; private set; }

    public User? User { get; set; }
    public Shortcut? Shortcut { get; set; }
    public string? Colour { get; private set; }

    protected UserShortcut() { }

    public UserShortcut(int userId, int shortcutId, int position, string colour = "var(--secondary-color)")
    {
        UserId = userId;
        ShortcutId = shortcutId;
        Position = position;
        Colour = colour;
    }

    public void UpdatePosition(int newPosition)
    {
        Position = newPosition;
    }

    public void UpdateColour(string colour)
    {
        Colour = Guard.Against.NullOrWhiteSpace(colour);
    }
}