using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Client.Api;

namespace Rise.Client.Pages.Events.Service
{
    public class EventService(TransportProvider _transport) : IEventService
    {
        public async Task<Result<EventResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default)
        {
           return (await _transport.Current.GetAsync<EventResponse.Detail>($"/api/events?{request.AsQuery()}", ctx))!;
        }

        public async Task<Result<EventResponse.Index>> GetCarouselAsync(CancellationToken ctx = default)
        {
           return (await _transport.Current.GetAsync<EventResponse.Index>("/api/events/carousel", cancellationToken: ctx))!;
        }

        public async Task<Result<EventResponse.DetailExtended>> GetEventById(GetByIdRequest.GetById request, CancellationToken ctx)
        {
           return (await _transport.Current.GetAsync<EventResponse.DetailExtended>($"/api/events/{request.Id}", cancellationToken: ctx))!;
        }
    }
}
