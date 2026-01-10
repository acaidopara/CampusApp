using Rise.Shared.News;
using Rise.Shared.Events;
using Rise.Shared.Common;

namespace Rise.Shared.Resto;

/// <summary>
/// Provides structures to facilitate structured queries for data retrieval scenarios.
/// This includes handling various pagination strategies such as skip/take and cursor-based
/// pagination, as well as supporting search terms, ordering, and additional filters
/// for flexible query compositions.
/// See https://medium.com/@otigasdev/pagination-done-right-a-dive-into-cursor-based-strategy-5d17dc284f10
/// </summary>
public static partial class SearchRequest
{
    /// <summary>
    /// Represents a request model to handle pagination and filtering scenarios. It includes parameters for
    /// skipping and taking records, ordering, search terms, and custom filters to enable flexible data queries.
    /// </summary>
    public class SkipTake : QueryRequest.SkipTake
    {

        public bool? IsVegan { get; set; } = false;
        public bool? IsVeggie { get; set; } = false;

        public new string AsQuery() => $"skip={Skip}&take={Take}&orderBy={OrderBy}&orderDescending={OrderDescending}&searchTerm={SearchTerm}&isVegan={IsVegan}&isVeggie={IsVeggie}";
    }


}