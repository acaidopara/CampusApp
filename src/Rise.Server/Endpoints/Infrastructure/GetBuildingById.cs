using Rise.Shared.Infrastructure;

namespace Rise.Server.Endpoints.Infrastructure;

public class GetBuildingById(IInfrastructureService infrastructureService) 
    : Endpoint<BuildingRequest.GetById, Result<BuildingResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/campus/{CampusId}/buildings/{BuildingId}");
        AllowAnonymous();
    }

    public override Task<Result<BuildingResponse.Detail>> ExecuteAsync(BuildingRequest.GetById req, CancellationToken ct)
    {        
        return infrastructureService.GetBuildingById(req, ct);
    }
}