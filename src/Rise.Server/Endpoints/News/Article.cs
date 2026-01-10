using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Server.Endpoints.News;

/// <summary>
/// GET /api/news/{id}
/// Return a single event article by id.
/// </summary>
public class Article(INewsService newsService) : Endpoint<GetByIdRequest.GetById, Result<NewsResponse.DetailExtended>>
{
    public override void Configure()
    {
        Get("/api/news/{id}");
        AllowAnonymous();
    }

    public override Task<Result<NewsResponse.DetailExtended>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return newsService.GetNewsById(req, ct);
    }
}