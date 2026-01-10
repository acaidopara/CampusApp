using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Dashboard;

public partial class NewsCarousel
{
    [Parameter] public IEnumerable<Rise.Shared.News.NewsDto.Index>? News { get; set; }
    [Parameter] public IEnumerable<Rise.Shared.Events.EventDto.Index>? Events { get; set; }
    [Parameter] public bool ShowIndicators { get; set; } = true;
}