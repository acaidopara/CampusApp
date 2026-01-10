using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Rise.Client.Pages.Shortcuts.Components;

public partial class QuicklinkItem
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string? Icon { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string LinkUrl { get; set; } = "#";
    [Parameter] public string? Colour { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    private async Task HandleClick(MouseEventArgs _)
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(null);
            return;
        }

        if (string.IsNullOrWhiteSpace(LinkUrl) || LinkUrl == "#")
            return;

        var isAbsolute = Uri.IsWellFormedUriString(LinkUrl, UriKind.Absolute);
        if (isAbsolute)
        {
            Nav.NavigateTo(LinkUrl, forceLoad: true);
            return;
        }

        Nav.NavigateTo(LinkUrl);
    }
}