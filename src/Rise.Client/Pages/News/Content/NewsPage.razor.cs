using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Rise.Client.Pages.News.Content;

public partial class NewsPage {
    private int _activeTabIndex;
    private int _lastActiveTabIndex = -1;
    private bool _isLoading = true;
    [Inject] public IJSRuntime JsRuntime { get; set; } = default!;

    protected override void OnInitialized()
    {
        var uri = new Uri(NavigationManager.Uri);
        var query = uri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(q => q.Split('='))
            .ToDictionary(kv => kv[0], kv => kv.Length > 1 ? kv[1] : "");

        if (query.TryGetValue("tab", out var tabValue) && int.TryParse(tabValue, out var idx))
        {
            _activeTabIndex = idx;
        }
        _isLoading = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_activeTabIndex != _lastActiveTabIndex)
        {
            _lastActiveTabIndex = _activeTabIndex;
            await JsRuntime.InvokeVoidAsync("window.scrollTo", 0, 0);
        }
    }

    private void HandleSwipeEnd(SwipeEventArgs e)
    {
        switch (e.SwipeDirection)
        {
            case SwipeDirection.RightToLeft:  
                if (_activeTabIndex < 1) _activeTabIndex++;
                break;
            case SwipeDirection.LeftToRight:  
                if (_activeTabIndex > 0) _activeTabIndex--;
                break;
        }
        StateHasChanged();
    }
}