using Rise.Shared.Common;

namespace Rise.Shared.Infrastructure;

/// <summary>
/// Defines the interface for infrastructure-related services.
/// This interface is shared between client and server layers to ensure consistent API contracts.
/// It includes methods for fetching paginated indexes of campuses, buildings, and classrooms.
/// </summary>
public interface IInfrastructureService
{
    /// <summary>
    /// Asynchronously retrieves a paginated index of campuses.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    Task<Result<CampusResponse.Index>> GetCampusesIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default);

    /// <summary>
    /// Asynchronously retrieves a paginated index of buildings.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    Task<Result<BuildingResponse.Index>> GetBuildingsIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default);

    /// <summary>
    /// Asynchronously retrieves a paginated index of classrooms.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    Task<Result<ClassroomResponse.Index>> GetClassroomsIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default);
    
    Task<Result<InfrastructureResponse.Index>> GetInfrastructureIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default);
    
    Task<Result<CampusResponse.Detail>> GetCampusById(GetByIdRequest.GetById request,
        CancellationToken ctx = default);
    
    Task<Result<BuildingResponse.Detail>> GetBuildingById(BuildingRequest.GetById request,
        CancellationToken ctx = default);
    
    Task<Result<ClassroomResponse.Index>> GetClassroomById(ClassroomRequest.GetById request,
        CancellationToken ctx = default);

    Task<Result<Resto.RestoResponse.Index>> GetRestosFromCampus(GetByIdRequest.GetById request, CancellationToken ctx = default);
}