using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Sorting;

public partial class SortToggle
{
    [Parameter] public string SortOption { get; set; } = "Descending";
    [Parameter] public string SortOptionTitle1 { get; set; } = "Ascending";
    [Parameter] public string SortOptionTitle2 { get; set; } = "Descending";
    [Parameter] public EventCallback<string> SortOptionChanged { get; set; }

    private async Task ToggleSort()
    {
        SortOption = SortOption == "Ascending"
            ? "Descending"
            : "Ascending";

        await SortOptionChanged.InvokeAsync(SortOption);
    }

    private string GetSortLabel()
    {
        return SortOption == "Ascending" ? SortOptionTitle1 : SortOptionTitle2;
    }

    private string GetSortTitle() => GetSortLabel();
}