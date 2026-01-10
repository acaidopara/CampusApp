namespace Rise.Shared.News;

/// <summary>
/// Contains data transfer objects (DTOs) used for news-related operations.
/// </summary>
public static class NewsDto
{
    /// <summary>
    /// Represents a minimal overview of a news article (for listings or carousels).
    /// </summary>
    public class Index
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string ImageUrl { get; set; }
    }
    
    /// <summary>
    /// Represents base detail news article information (for article detail list).
    /// </summary>
    public class Detail : Index
    {
        public required string Subject { get; set; }
        public required DateTime Date { get; set; }
        public required string Content { get; set; }
    }

    /// <summary>
    /// Represents detailed news article information (for article detail page).
    /// </summary>
    public class DetailExtended : Detail
    {
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorFunction { get; set; } = string.Empty;
        public string? AuthorAvatarUrl { get; set; }
    }
}