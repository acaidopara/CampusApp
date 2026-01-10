using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.MainComponents.Filtering;

public partial class FilterChip
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string? Icon { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public int? UnreadCount { get; set; }
    private Color ChipColor => IsActive ? Color.Primary : Color.Default;
    private Variant ChipVariant => IsActive ? Variant.Filled : Variant.Outlined;


    private async Task OnClickHandler()
    {
        await OnClick.InvokeAsync();
    }
}