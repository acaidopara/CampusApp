using Rise.Shared.Common;

namespace Rise.Shared.Support;

public interface ISupportService
{ 
    Task<Result<SupportResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    Task<Result<SupportResponse.ByName>> GetByNameAsync(string name, CancellationToken ctx = default);
}