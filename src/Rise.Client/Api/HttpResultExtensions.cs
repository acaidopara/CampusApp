using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Rise.Client.Api;
public static class HttpResultExtensions
{
    public static async Task<Result<T?>> ReadResultAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Error($"HTTP {response.StatusCode}");
        }

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Error("Server returned an empty body.");
        }

        try
        {
            var result = JsonSerializer.Deserialize<Result<T?>>(raw);

            return result ?? Result.Error("Failed to parse JSON response.");
        }
        catch (Exception ex)
        {
            return Result.Error($"Invalid JSON response: {ex.Message}");
        }
    }

     public static async Task<Result> ReadResultAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
            return Result.Error($"HTTP {response.StatusCode}");

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(raw))
            return Result.Error("Server returned an empty body.");

        try
        {
            var result = JsonSerializer.Deserialize<Result>(raw);
            return result ?? Result.Error("Failed to parse JSON response.");
        }
        catch (Exception ex)
        {
            return Result.Error($"Invalid JSON response: {ex.Message}");
        }
    }
}
