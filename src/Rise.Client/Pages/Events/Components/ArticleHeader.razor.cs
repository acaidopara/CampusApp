using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Rise.Shared.Events;

namespace Rise.Client.Pages.Events.Components;

public partial class ArticleHeader
{
    [Parameter] public EventDto.Detail? Article { get; set; }

    private async Task CopyLink()
    {
        if (Article == null) return;

        var url = NavigationManager.Uri;
        await Js.InvokeVoidAsync("navigator.clipboard.writeText", url);

        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add(Loc["LinkCopied"].Value, Severity.Success, config =>
        {
            config.VisibleStateDuration = 1500;
            config.ShowCloseIcon = false;
        });
    }
}