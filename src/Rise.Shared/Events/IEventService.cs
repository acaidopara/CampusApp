using Rise.Shared.Common;

namespace Rise.Shared.Events;

public interface IEventService
{
    Task<Result<EventResponse.Index>> GetCarouselAsync(CancellationToken ctx = default);
    Task<Result<EventResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default);
    Task<Result<EventResponse.DetailExtended>> GetEventById(GetByIdRequest.GetById request, CancellationToken ctx = default);
}