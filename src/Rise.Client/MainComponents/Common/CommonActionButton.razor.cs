using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Common;

public partial class CommonActionButton
{
    [Parameter] public string Text { get; set; } = "";
    [Parameter] public EventCallback OnClick { get; set; }
}