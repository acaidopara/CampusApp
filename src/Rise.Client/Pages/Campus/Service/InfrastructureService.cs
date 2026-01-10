using System.Net.Http.Json;
using Rise.Client.Api;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Shared.Resto;

namespace Rise.Client.Pages.Campus.Service;

/// <summary>
/// Client-side service for interacting with infrastructure-related API endpoints.
/// This service handles fetching paginated indexes of campuses, buildings, classrooms, and restos.
/// </summary>
public class InfrastructureService(TransportProvider _transport) : IInfrastructureService
{
    /// <summary>
    /// Asynchronously retrieves a paginated index of campuses.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    public async Task<Result<CampusResponse.Index>> GetCampusesIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default)
    {
        var queryParams = $"?searchTerm={Uri.EscapeDataString(request.SearchTerm)}" +
                          $"&skip={request.Skip}" +
                          $"&take={request.Take}" +
                          $"&orderBy={Uri.EscapeDataString(request.OrderBy ?? string.Empty)}" +
                          $"&orderDescending={request.OrderDescending}";

        var url = $"/api/campuses{queryParams}";

        return (await _transport.Current.GetAsync<CampusResponse.Index>(url, ctx))!;
    }

    /// <summary>
    /// Asynchronously retrieves a paginated index of buildings.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    public async Task<Result<BuildingResponse.Index>> GetBuildingsIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default)
    {
        var queryParams = $"?searchTerm={Uri.EscapeDataString(request.SearchTerm)}" +
                          $"&skip={request.Skip}" +
                          $"&take={request.Take}" +
                          $"&orderBy={Uri.EscapeDataString(request.OrderBy ?? string.Empty)}" +
                          $"&orderDescending={request.OrderDescending}";

        var url = $"/api/buildings{queryParams}";

        return (await _transport.Current.GetAsync<BuildingResponse.Index>(url, ctx))!;
    }

    /// <summary>
    /// Asynchronously retrieves a paginated index of classrooms.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    public async Task<Result<ClassroomResponse.Index>> GetClassroomsIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default)
    {
        var queryParams = $"?searchTerm={Uri.EscapeDataString(request.SearchTerm)}" +
                          $"&skip={request.Skip}" +
                          $"&take={request.Take}" +
                          $"&orderBy={Uri.EscapeDataString(request.OrderBy ?? string.Empty)}" +
                          $"&orderDescending={request.OrderDescending}";

        var url = $"/api/classrooms{queryParams}";

        return (await _transport.Current.GetAsync<ClassroomResponse.Index>(url, ctx))!;
    }

    /// <summary>
    /// Asynchronously retrieves a combined paginated index of all infrastructure (campuses, buildings, classrooms, restos).
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the combined index response.</returns>
    public async Task<Result<InfrastructureResponse.Index>> GetInfrastructureIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var campusesTask = GetCampusesIndexAsync(request, ctx);
        var buildingsTask = GetBuildingsIndexAsync(request, ctx);
        var classroomsTask = GetClassroomsIndexAsync(request, ctx);
        //var restosTask = GetRestosIndexAsync(request, ctx);

        await Task.WhenAll(campusesTask, buildingsTask, classroomsTask/*, restosTask*/);

        var campusesResult = await campusesTask;
        var buildingsResult = await buildingsTask;
        var classroomsResult = await classroomsTask;
        //var restosResult = await restosTask;

        if (!campusesResult.IsSuccess || !buildingsResult.IsSuccess || !classroomsResult.IsSuccess /*|| !restosResult.IsSuccess*/)
        {
            return Result.Conflict("Failed to fetch one or more infrastructure indexes.");
        }


        return Result.Success(new InfrastructureResponse.Index
        {
            Campuses = campusesResult.Value,
            Buildings = buildingsResult.Value,
            Classrooms = classroomsResult.Value,
            //Restos = restosResult.Value
        });
    }

    public async Task<Result<CampusResponse.Detail>> GetCampusById(GetByIdRequest.GetById request,
        CancellationToken ctx = default)
    {
        return (await _transport.Current.GetAsync<CampusResponse.Detail>($"/api/campus/{request.Id}", cancellationToken: ctx))!;
    }

    public async Task<Result<BuildingResponse.Detail>> GetBuildingById(BuildingRequest.GetById request,
        CancellationToken ctx = default)
    {
        return (await _transport.Current.GetAsync<BuildingResponse.Detail>($"/api/campus/{request.CampusId}/buildings/{request.BuildingId}", cancellationToken: ctx))!;
    }

    public async Task<Result<ClassroomResponse.Index>> GetClassroomById(ClassroomRequest.GetById request, CancellationToken ctx = default)
    {
        return (await _transport.Current.GetAsync<ClassroomResponse.Index>($"/api/campus/{request.CampusId}/building/{request.BuildingId}/classroom/{request.ClassroomId}", ctx))!;
    }

    public async Task<Result<RestoResponse.Index>> GetRestosFromCampus(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        return (await _transport.Current.GetAsync<RestoResponse.Index>($"/api/campus/{request.Id}/restos", cancellationToken: ctx))!;
    }
}