using Rise.Shared.Common;
using Rise.Shared.Departments;
using Rise.Shared.Events;
using Rise.Shared.Resto;

namespace Rise.Server.Endpoints.Events;

/// <summary>
/// List all departments.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="eventService"></param>
public class Index(IEventService eventService) : Endpoint<TopicRequest.GetBasedOnTopic, Result<EventResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/events");
        AllowAnonymous(); 
    }

    public override Task<Result<EventResponse.Detail>> ExecuteAsync(TopicRequest.GetBasedOnTopic req, CancellationToken ct)
    {
        return eventService.GetIndexAsync(req, ct);
    }
}