using Rise.Shared.Deadlines;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Deadlines;

/// <summary>
/// Endpoint for retrieving a paginated index of deadlines for the authenticated student.
/// This GET endpoint supports search, pagination, and ordering.
/// Utilizes FastEndpoints for request handling and validation.
/// See https://fast-endpoints.com/ for more details on the framework.
/// </summary>
/// <param name="deadlineService">Injected service for deadline operations.</param>
public class Index(IDeadlineService deadlineService) : Endpoint<DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and HTTP method.
    /// Maps to GET /api/deadlines with query parameters for filtering and pagination.
    /// </summary>
    public override void Configure()
    {
        Get("/api/deadlines");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Executes the endpoint logic asynchronously.
    /// Delegates to the deadline service to fetch the index of deadlines.
    /// </summary>
    /// <param name="req">The request object containing search, skip, take, orderBy, and orderDescending.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A Result containing the index response or an error.</returns>
    public override Task<Result<DeadlineResponse.Index>> ExecuteAsync(DeadlineRequest.GetForStudent req, CancellationToken ct)
    {
        return deadlineService.GetIndexAsync(req, ct);
    }
}