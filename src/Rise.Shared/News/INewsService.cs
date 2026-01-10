using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Shared.News;

/// <summary>
/// Provides methods for managing news-related operations.
/// </summary>
public interface INewsService
{
    Task<Result<NewsResponse.Index>> GetCarouselAsync(CancellationToken ctx = default);
    Task<Result<NewsResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default);
    Task<Result<NewsResponse.DetailExtended>> GetNewsById(GetByIdRequest.GetById request, CancellationToken ctx = default);
}