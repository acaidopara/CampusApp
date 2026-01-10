using Microsoft.AspNetCore.Components;

namespace Rise.Client.Layout;

public partial class TopPage
{
    [Parameter] public string BackLinkUrl { get; set; } = "#";
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string? Subtitle { get; set; }
}