using Rise.Shared.Shortcuts;

namespace Rise.Domain.Dashboard;

public class Shortcut : Entity
{
    private string? _title = string.Empty;
    public string? Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string? _icon = string.Empty;
    public string? Icon
    {
        get => _icon;
        set => _icon = value;
    }
    
    private string? _label = string.Empty;
    public string? Label
    {
        get => _label;
        set => _label = value;
    }
    
    private string? _linkUrl = string.Empty;
    public string? LinkUrl
    {
        get => _linkUrl;
        set => _linkUrl = Guard.Against.NullOrWhiteSpace(value);
    }
    
    public ShortcutType ShortcutType { get; set; }
    
    public bool DefaultForGuest { get; set; }
    
    public ICollection<UserShortcut> UserShortcuts { get; set; } = [];

    protected Shortcut()
    {
        
    }
    
    public Shortcut(string title, ShortcutType shortcutType, string icon, string label, string linkUrl, bool defaultForGuest)
    {
        Title = title;
        ShortcutType = shortcutType;
        Icon = icon;
        Label = label;
        LinkUrl = linkUrl;
        DefaultForGuest = defaultForGuest;
    }
}