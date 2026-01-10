using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Filtering;

public partial class Filtering
{
    private string _searchTerm = "";
    [Parameter] public List<FilterChipSet.FilterItem> Items { get; set; } = [];
    [Parameter] public string? SelectedItem { get; set; }
    [Parameter] public EventCallback<string?> SelectedItemChanged { get; set; }
    [Parameter] public EventCallback<string> SearchTermChanged { get; set; }
    [Parameter] public required string Placeholder { get; set; }
    [Parameter] public int? UnreadCount { get; set; }

    private async Task OnSearchChangedInternal(string value)
    {
        _searchTerm = value;
        await SearchTermChanged.InvokeAsync(value);
    }
}