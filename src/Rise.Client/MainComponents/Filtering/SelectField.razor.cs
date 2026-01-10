using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Filtering;

public partial class SelectField
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback<string?> OnChange { get; set; }
    [Parameter] public List<string> Items { get; set; } = [];
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? Css { get; set; }

    private async Task OnValueChanged(string? newValue)
    {
        Value = newValue;
        await OnChange.InvokeAsync(Value);
        await ValueChanged.InvokeAsync(Value);
    }
}