using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Common;

public partial class CommonSwitch
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public string Label { get; set; } = "";
    [Parameter] public bool WithBorder { get; set; } = true;

    private async Task Toggle()
    {
        Checked = !Checked;
        await CheckedChanged.InvokeAsync(Checked);
    }
}