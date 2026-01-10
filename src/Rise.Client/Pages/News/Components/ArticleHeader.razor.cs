using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Rise.Shared.News;

namespace Rise.Client.Pages.News.Components;

public partial class ArticleHeader
{
    [Parameter] public NewsDto.DetailExtended? Article { get; set; }
    private int ReadTime => CalculateReadTime(Article?.Content ?? string.Empty);

    private async Task CopyLink()
    {
        if (Article == null) return;

        var url = NavigationManager.Uri;
        await Js.InvokeVoidAsync("navigator.clipboard.writeText", url);

        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add(Loc["LinkCopied"], Severity.Success, config =>
        {
            config.VisibleStateDuration = 1500;
            config.ShowCloseIcon = false;
        });
    }

    private int CalculateReadTime(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var wordCount = text.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
        const int wordsPerMinute = 225;
        return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
    }
}