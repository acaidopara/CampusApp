using Rise.Shared.Common;
using Rise.Shared.Infrastructure;

namespace Rise.Server.Endpoints.Infrastructure;

public class GetCampusById(IInfrastructureService infrastructureService) 
    : Endpoint<GetByIdRequest.GetById, Result<CampusResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/campus/{Id}");
        AllowAnonymous();
    }

    public override Task<Result<CampusResponse.Detail>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {        
        return infrastructureService.GetCampusById(req, ct);
    }
}