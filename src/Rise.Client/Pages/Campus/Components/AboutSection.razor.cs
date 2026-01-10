using Microsoft.AspNetCore.Components;

namespace Rise.Client.Pages.Campus.Components;

public partial class AboutSection
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public int MaxLength { get; set; } = 200;
    [Parameter] public string ReadMoreText { get; set; } = "Read more";
    [Parameter] public string ReadLessText { get; set; } = "Read less";
    private bool ShowFull { get; set; } = false;

    private string ShortDescription =>
        Description.Length > MaxLength ? Description.Substring(0, MaxLength) + "..." : Description;

    private void ToggleShowFull()
    {
        ShowFull = !ShowFull;
    }
}