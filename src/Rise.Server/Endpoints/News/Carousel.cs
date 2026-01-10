using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Server.Endpoints.News;

/// <summary>
/// GET /api/news/carousel
/// Return a small set of events for the homepage carousel.
/// </summary>
public class Carousel(INewsService newsService) : EndpointWithoutRequest<Result<NewsResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/news/carousel");
        AllowAnonymous();
    }

    public override Task<Result<NewsResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return newsService.GetCarouselAsync(ct);
    }
}