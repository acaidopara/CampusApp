using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Api;

namespace Rise.Client.Api;

public class ApiClient : IAppService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private readonly ILocalStorageService _localStorage;

    public ApiClient(HttpClient http, NavigationManager nav, ILocalStorageService cache)
    {
        _http = http;
        _nav = nav;
        _localStorage = cache;
    }

    public async Task<Result<T?>> GetAsync<T>(string url, CancellationToken cancellationToken = default, bool saveToCache = true)
    {
        var response = await _http.GetAsync(url, cancellationToken);
        var res = await response.Content.ReadFromJsonAsync<Result<T>>(cancellationToken: cancellationToken);
        if (res != null && saveToCache)
            await _localStorage.SetItemAsync(url, res);
        return res!;
    }

    public async Task<Result<T?>> PostAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PostAsJsonAsync(url, request, cancellationToken);
        return await response.ReadResultAsync<T>(cancellationToken: cancellationToken)!;
    }

    public async Task<Result<T?>> DelAsync<T>(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.DeleteAsync(url, cancellationToken);
        return await response.ReadResultAsync<T>(cancellationToken: cancellationToken)!;
    }
    public async Task<Result<T?>> PutAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PutAsJsonAsync(url, request, cancellationToken);
        return await response.ReadResultAsync<T>(cancellationToken: cancellationToken)!;
    }
    public async Task<Result<T?>> PatchAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PatchAsJsonAsync(url, request, cancellationToken);
        return await response.ReadResultAsync<T>(cancellationToken: cancellationToken)!;
    }

    public async Task<Result> PostAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PostAsJsonAsync(url, request, cancellationToken);
        return await response.ReadResultAsync(cancellationToken: cancellationToken)!;
    }

    public async Task<Result> PutAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PutAsJsonAsync(url, request, cancellationToken);
        return await response.ReadResultAsync(cancellationToken: cancellationToken)!;
    }

    public Task ShutDown()
    {
        return Task.CompletedTask;
    }

    public async Task<Result> PostAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.PostAsJsonAsync<object?>(url, null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return Result.NoContent();

        if (!response.Content.Headers.ContentLength.HasValue ||
            response.Content.Headers.ContentLength == 0)
        {
            return Result.NoContent();
        }
        return await response.ReadResultAsync(cancellationToken: cancellationToken);

    }

    public async Task<Result> DelAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        var response = await _http.DeleteAsync(url, cancellationToken);
        return await response.ReadResultAsync(cancellationToken: cancellationToken)!;
    }
}
