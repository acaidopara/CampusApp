namespace Rise.Shared.News;

/// <summary>
/// Represents the response structure for news-related operations.
/// </summary>
public static class NewsResponse
{
    public class Index
    {
        public required IEnumerable<NewsDto.Index> News { get; init; }
    }
    
    public class Detail
    {
        public required IEnumerable<NewsDto.Detail> News { get; init; }
        public int TotalCount { get; init; }
    }
    
    public class DetailExtended
    {
        public NewsDto.DetailExtended News { get; init; } = null!;
    }
}