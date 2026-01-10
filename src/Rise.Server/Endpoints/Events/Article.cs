using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Server.Endpoints.Events;

/// <summary>
/// GET /api/events/{id}
/// Return a single event article by id.
/// </summary>
public class Article(IEventService eventService) : Endpoint<GetByIdRequest.GetById, Result<EventResponse.DetailExtended>>
{
    public override void Configure()
    {
        Get("/api/events/{id}");
        AllowAnonymous();
    }

    public override Task<Result<EventResponse.DetailExtended>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return eventService.GetEventById(req, ct);
    }
}