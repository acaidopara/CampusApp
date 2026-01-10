using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Server.Endpoints.Events;

/// <summary>
/// GET /api/events/carousel
/// Return a small set of events for the homepage carousel.
/// </summary>
public class Carousel(IEventService eventService) : EndpointWithoutRequest<Result<EventResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/events/carousel");
        AllowAnonymous();
    }

    public override Task<Result<EventResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return eventService.GetCarouselAsync(ct);
    }
}