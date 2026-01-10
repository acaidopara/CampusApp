using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Filtering;

public partial class FilterChipSet
{
    public record FilterItem(string Label, string? Icon);

    [Parameter] public List<FilterItem> Items { get; set; } = [];
    [Parameter] public string? SelectedItem { get; set; }
    [Parameter] public EventCallback<string?> SelectedItemChanged { get; set; }
    [Parameter] public int? UnreadCount { get; set; }

    protected override void OnInitialized()
    {
        if (SelectedItem == null && Items.Any())
            SelectedItem = Items[0].Label;
    }

    private async Task SelectItem(string label)
    {
        SelectedItem = (SelectedItem == label) ? null : label;
        await SelectedItemChanged.InvokeAsync(SelectedItem);
    }
}