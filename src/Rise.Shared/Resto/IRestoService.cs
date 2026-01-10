using Rise.Shared.Common;
using Rise.Shared.Infrastructure;

namespace Rise.Shared.Resto;

/// <summary>
/// Provides methods for managing resto-related operations.
/// </summary>
public interface IRestoService
{
    Task<Result<Resto.RestoResponse.Index>> GetIndexAsync(SearchRequest.SkipTake request, CancellationToken ctx = default);
}