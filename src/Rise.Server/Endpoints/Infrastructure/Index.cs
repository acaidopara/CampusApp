using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Shared.Resto;

namespace Rise.Server.Endpoints.Infrastructure;

public class CampusesIndex(IInfrastructureService infrastructureService) : Endpoint<QueryRequest.SkipTake, Result<CampusResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/campuses");
        AllowAnonymous(); 
    }

    public override Task<Result<CampusResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return infrastructureService.GetCampusesIndexAsync(req, ct);
    }
}

public class BuildingsIndex(IInfrastructureService infrastructureService) : Endpoint<QueryRequest.SkipTake, Result<BuildingResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/buildings");
        AllowAnonymous(); 
    }

    public override Task<Result<BuildingResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return infrastructureService.GetBuildingsIndexAsync(req, ct);
    }
}

public class ClassroomsIndex(IInfrastructureService infrastructureService) : Endpoint<QueryRequest.SkipTake, Result<ClassroomResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/classrooms");
        AllowAnonymous(); 
    }

    public override Task<Result<ClassroomResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return infrastructureService.GetClassroomsIndexAsync(req, ct);
    }
}

public class GetRestosFromCampus(IInfrastructureService infrastructureService) : Endpoint<GetByIdRequest.GetById, Result<RestoResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/campus/{id}/restos");
        AllowAnonymous(); 
    }

    public override Task<Result<RestoResponse.Index>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return infrastructureService.GetRestosFromCampus(req, ct);
    }
}