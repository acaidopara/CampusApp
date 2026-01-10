using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Filtering;

public partial class SearchBar
{
    [Parameter] public string SearchTerm { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> SearchTermChanged { get; set; }
    [Parameter] public required string PlaceHolderText { get; set; }

    private async Task OnSearchChanged(string value)
    {
        if (SearchTerm != value)
        {
            SearchTerm = value;
            await SearchTermChanged.InvokeAsync(value);
        }
    }
}