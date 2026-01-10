using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Client.Pages.Events.Content;

public partial class EventArticlePage
{
    [Parameter] public int EventId { get; set; }
    private EventDto.DetailExtended? _article;

    protected override async Task OnInitializedAsync()
    {
        var result = await EventService.GetEventById(new GetByIdRequest.GetById
        {
            Id = EventId
        });
        _article = result.IsSuccess ? result.Value.Event : null;
    }
}