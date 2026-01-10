using MudBlazor;

namespace Rise.Client.MainComponents.Navigation;

public partial class BottomNav
{
    private record NavItem(string Label, string Href, string IconOutlined, string IconFilled);

    private readonly List<NavItem> _navItems =
    [
        new("Home", "", Icons.Material.Outlined.Home, Icons.Material.Filled.Home),
        new("News", "nieuws-en-events", Icons.Material.Outlined.Article, Icons.Material.Filled.Article),
        new("Campus", "campus", Icons.Material.Outlined.Explore, Icons.Material.Filled.Explore),
        new("Timetable", "lessenrooster", Icons.Material.Outlined.CalendarMonth, Icons.Material.Filled.CalendarMonth),
        new("Deadlines", "deadlines", Icons.Material.Outlined.Checklist, Icons.Material.Filled.Checklist)
    ];
}