using Rise.Shared.News;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Server.Endpoints.News
{
    /// <summary>
    /// List of all news.
    /// See https://fast-endpoints.com/
    /// </summary>
    /// <param name="newsService"></param>
    public class Index(INewsService newsService) : Endpoint<TopicRequest.GetBasedOnTopic, Result<NewsResponse.Detail>>
    {
        public override void Configure()
        {
            Get("/api/news");
            AllowAnonymous();
        }

        public override Task<Result<NewsResponse.Detail>> ExecuteAsync(TopicRequest.GetBasedOnTopic req, CancellationToken ct)
        {
            return newsService.GetIndexAsync(req, ct);
        }
    }
}
