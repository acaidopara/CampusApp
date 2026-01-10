using Rise.Shared.Common;
using Rise.Shared.Departments;
using Rise.Shared.Resto;

namespace Rise.Server.Endpoints.Resto;

/// <summary>
/// List all restos.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="restoService"></param>
public class Index(IRestoService restoService) : Endpoint<SearchRequest.SkipTake, Result<RestoResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/restos");
        AllowAnonymous(); 
    }

    public override Task<Result<RestoResponse.Index>> ExecuteAsync(SearchRequest.SkipTake req, CancellationToken ct)
    {
        return restoService.GetIndexAsync(req, ct);
    }
}