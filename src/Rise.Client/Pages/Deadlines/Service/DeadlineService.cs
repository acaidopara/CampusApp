using System.Net.Http.Json;
using Rise.Shared.Deadlines;
using Rise.Client.Api;

namespace Rise.Client.Pages.Deadlines.Service;

/// <summary>
/// Client-side service for interacting with deadline-related API endpoints.
/// This service handles fetching deadlines and toggling their completion status.
/// </summary>
public class DeadlineService(TransportProvider _transport) : IDeadlineService
{
    /// <summary>
    /// Retrieves a paginated index of deadlines for the current student.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A result containing the index of deadlines or an error.</returns>
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(DeadlineRequest.GetForStudent request, CancellationToken ct = default)
    {
        var queryParams = $"?searchTerm={Uri.EscapeDataString(request.SearchTerm ?? string.Empty)}" +
                          $"&skip={request.Skip}" +
                          $"&take={request.Take}" +
                          $"&orderBy={Uri.EscapeDataString(request.OrderBy ?? string.Empty)}" +
                          $"&orderDescending={request.OrderDescending}" +
                          $"&startDate={Uri.EscapeDataString(request.StartDate ?? string.Empty)}" +
                          $"&endDate={Uri.EscapeDataString(request.EndDate ?? string.Empty)}";

        var url = $"/api/deadlines{queryParams}";

        return (await _transport.Current.GetAsync<DeadlineResponse.Index>(url, ct))!;
    }

    /// <summary>
    /// Toggles the completion status of a specific deadline for the current student.
    /// </summary>
    /// <param name="request">The request containing the deadline ID and new completion status.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A result indicating success or failure of the operation.</returns>
    public async Task<Result> ToggleCompletionAsync(DeadlineRequest.ToggleCompletion request, CancellationToken ct = default)
    {
        // Build the API URL for the specific deadline
        var url = $"/api/deadlines/{request.DeadlineId}/completion";

        // Create JSON content for the PATCH request
        var content = JsonContent.Create(new { request.IsCompleted });

        // Send PATCH request to update the status
        return (await _transport.Current.PatchAsync<Result,JsonContent>(url, content, ct))!;
    }
}