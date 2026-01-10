using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Common;

public partial class CommonLinkButton
{
    [Parameter] public string Text { get; set; } = "";
    [Parameter] public string Href { get; set; } = "#";
    [Parameter] public string Target { get; set; } = "_self";
    [Parameter] public string Rel { get; set; } = "noopener noreferrer";
    [Parameter] public string? Icon { get; set; }
}