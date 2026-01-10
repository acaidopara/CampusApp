using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.News;
using Rise.Client.Api;

namespace Rise.Client.Pages.News.Service
{
    public class NewsService(TransportProvider _transport) : INewsService
    {
        public async Task<Result<NewsResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default)
        {
            return (await _transport.Current.GetAsync<NewsResponse.Detail>($"/api/news?{request.AsQuery()}", ctx))!;
        }

        public async Task<Result<NewsResponse.Index>> GetCarouselAsync(CancellationToken ctx = default)
        {
            return (await _transport.Current.GetAsync<NewsResponse.Index>("/api/news/carousel", cancellationToken: ctx))!;
        }

        public async Task<Result<NewsResponse.DetailExtended>> GetNewsById(GetByIdRequest.GetById request, CancellationToken ctx)
        {
            return (await _transport.Current.GetAsync<NewsResponse.DetailExtended>($"/api/news/{request.Id}", cancellationToken: ctx))!;
        }
    }
}