using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Rise.Shared.CampusLife.StudentDeals;

namespace Rise.Client.Pages.CampusLife.Components;

public partial class DealItemHeader
{
    [Parameter] public StudentDealDto.Detail? Item { get; set; }
    [Parameter] public string Page { get; set; } = "#";

    private async Task CopyLink()
    {
        if (Item == null) return;

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