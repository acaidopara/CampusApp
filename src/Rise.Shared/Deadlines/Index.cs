namespace Rise.Shared.Deadlines;

/// <summary>
/// Static utility class containing response models for deadline operations.
/// These models define the structure of outgoing responses from service methods.
/// Optimized to include only actively used models (e.g., Index for paginated results).
/// </summary>
public static partial class DeadlineResponse
{
    /// <summary>
    /// Response model for paginated index of deadlines.
    /// Includes the list of deadlines and total count for pagination.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// The collection of deadline DTOs in the current page.
        /// </summary>
        public IEnumerable<DeadlineDto.Index> Deadlines { get; set; } = [];
        
        /// <summary>
        /// The total number of deadlines matching the query (for pagination).
        /// </summary>
        public int TotalCount { get; set; }
    }
}