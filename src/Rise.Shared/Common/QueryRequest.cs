namespace Rise.Shared.Common;

/// <summary>
/// Provides structures to facilitate structured queries for data retrieval scenarios.
/// This includes handling various pagination strategies such as skip/take and cursor-based
/// pagination, as well as supporting search terms, ordering, and additional filters
/// for flexible query compositions.
/// See https://medium.com/@otigasdev/pagination-done-right-a-dive-into-cursor-based-strategy-5d17dc284f10
/// </summary>
public static partial class QueryRequest
{
    /// <summary>
    /// Represents a request model to handle pagination and filtering scenarios. It includes parameters for
    /// skipping and taking records, ordering, search terms, and custom filters to enable flexible data queries.
    /// </summary>
    public class SkipTake
    {
        /// <summary>
        /// Gets or sets the term used for searching, enabling filtering of results based on text matches
        /// in relevant fields such as names or descriptions.
        /// </summary>
        public string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of records to skip in the query, enabling pagination by
        /// determining the starting point for the returned dataset.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the number of records to retrieve in a paginated query. This determines the maximum number
        /// of items to fetch from the data source in a single request.
        /// </summary>
        public int Take { get; set; } = 50;

        /// <summary>
        /// Gets or sets the property name by which the results should be ordered,
        /// allowing for customizable sorting of query outputs.
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ordering of the query results
        /// should be in descending order.
        /// </summary>
        public bool OrderDescending { get; set; }

        /// <summary>
        /// Gets or sets a collection of key-value pairs representing additional filtering criteria
        /// to further refine query results based on specified conditions.
        /// </summary>
        public Dictionary<string, object?> Filters { get; set; } = new();
        
        public string AsQuery() => $"skip={Skip}&take={Take}&orderBy={OrderBy}&orderDescending={OrderDescending}&searchTerm={SearchTerm}";
    }

    /// <summary>
    /// Represents a query request using a cursor-based pagination strategy,
    /// supporting search, ordering, and filtering. Cursor-based pagination
    /// allows for efficient and scalable data retrieval by using a key to mark
    /// the position in the dataset instead of relying on page numbers.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key used to represent the position in the dataset,
    /// typically corresponding to a unique field in the data (e.g., an ID or timestamp).
    /// </typeparam>
    public class Cursor<TKey>
    {
        /// <summary>
        /// Gets or sets the search term used to filter results based on partial or full text matches.
        /// </summary>
        public string SearchTerm { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the last key used to identify the starting point
        /// for the next page of results in a cursor-based pagination strategy.
        /// </summary>
        public TKey? LastKey { get; set; }

        /// <summary>
        /// Gets or sets the number of items to include per page in a paginated query.
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the property used to determine the sorting order of the query results.
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order of results should be descending.
        /// If set to true, results will be sorted in descending order based on the specified column or key.
        /// If set to false, results will be sorted in ascending order.
        /// </summary>
        public bool OrderDescending { get; set; }

        /// <summary>
        /// Gets or sets the collection of additional filters to refine query results.
        /// Each filter is represented as a key-value pair where the key is the filter name
        /// and the value specifies the filter criteria.
        /// </summary>
        public Dictionary<string, object?> Filters { get; set; } = new();
    }
}

