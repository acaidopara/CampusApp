using Rise.Shared.Common;
using Rise.Shared.Infrastructure;

namespace Rise.Server.Endpoints.Infrastructure;

public class GetClassroomById(IInfrastructureService infrastructureService) 
    : Endpoint<ClassroomRequest.GetById, Result<ClassroomResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/campus/{CampusId}/building/{BuildingId}/classroom/{ClassroomId}");
        AllowAnonymous();
    }

    public override Task<Result<ClassroomResponse.Index>> ExecuteAsync(ClassroomRequest.GetById req, CancellationToken ct)
    {        
        return infrastructureService.GetClassroomById(req, ct);
    }
}