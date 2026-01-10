using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using MudBlazor.Utilities;

namespace Rise.Client.Layout;

public partial class MainLayout
{
    private bool _showTopNav = true;
    private bool _showBottomNav = true;

    private readonly MudTheme _myCustomTheme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = new MudColor("#000000"),
            Secondary = new MudColor("#FABC32"),
            AppbarBackground = new MudColor("#000000")
        },
        PaletteDark = new PaletteDark()
        {
            Primary = new MudColor("#000000"),
            Secondary = new MudColor("#FABC32")
        },
        LayoutProperties = new LayoutProperties()
        {
            AppbarHeight = "64px"
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = ["Montserrat", "sans-serif"]
            },
        }
    };

    protected override void OnInitialized()
    {
        Nav.LocationChanged += HandleLocationChanged;
        UpdateNavVisibility(Nav.Uri);
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        UpdateNavVisibility(e.Location);
        StateHasChanged();
    }

    private void UpdateNavVisibility(string uri)
    {
        // Enter the links here to hide the top navigation bar
        _showTopNav = !(
            uri.Contains("/login", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/vacatures/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/studenten-deals/", StringComparison.OrdinalIgnoreCase) ||
            (uri.Contains("/campus/", StringComparison.OrdinalIgnoreCase) &&
             uri.Contains("/detail", StringComparison.OrdinalIgnoreCase)) ||
            (uri.Contains("/campus/", StringComparison.OrdinalIgnoreCase) && 
             !uri.Contains("/building", StringComparison.OrdinalIgnoreCase) && 
             !uri.Contains("/classroom", StringComparison.OrdinalIgnoreCase)) ||
            uri.Contains("/instellingen", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/it-support", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/snelkoppelingen", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/contact", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/studentenkaart", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/notificaties/notificatie/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/nieuws-en-events/nieuws/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/nieuws-en-events/events/", StringComparison.OrdinalIgnoreCase)
        );

        // Enter the links here to hide the bottom navigation bar
        _showBottomNav = !(
            uri.Contains("/nieuws-en-events/nieuws/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/nieuws-en-events/events/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/notificaties/notificatie/", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/login", StringComparison.OrdinalIgnoreCase) ||
            (uri.Contains("/campus/", StringComparison.OrdinalIgnoreCase) && uri.Contains("/detail", StringComparison.OrdinalIgnoreCase)) ||
            (uri.Contains("/campus/", StringComparison.OrdinalIgnoreCase) && !uri.Contains("/building", StringComparison.OrdinalIgnoreCase) && !uri.Contains("/classroom", StringComparison.OrdinalIgnoreCase)) ||
            uri.Contains("/resto", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/studenten-deals", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/vacatures", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/studentenverenigingen", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/instellingen", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/it-support", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/snelkoppelingen", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/contact", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/studentenkaart", StringComparison.OrdinalIgnoreCase) ||
            uri.Contains("/notificaties", StringComparison.OrdinalIgnoreCase)
        );
    }

    public void Dispose()
    {
        Nav.LocationChanged -= HandleLocationChanged;
    }
}