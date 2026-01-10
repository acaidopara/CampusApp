using Microsoft.AspNetCore.Components;

namespace Rise.Client.Layout;

public partial class LetterHeader
{
    [Parameter] public string ImageName { get; set; } = "image1.jpg";
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string? BackLinkUrl { get; set; }
}