
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.Serialization.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using Rise.Shared.Api;

namespace Rise.Client.Api;

public class OfflineService : IAppService
{
    private readonly ILocalStorageService _localStorage;
    private readonly List<IQueuedRequest> queuedRequests = new();
    private readonly HttpClient _http;

    public OfflineService(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _http = httpClient;

    }

    public Task ClearCache()
    {
        _localStorage.ClearAsync();
        return Task.CompletedTask;
    }

    public async Task SendQueuedRequests()
    {
        foreach (var req in queuedRequests)
        {
            switch (req.Method.Method)
            {
                case "POST":
                    await _http.PostAsJsonAsync(req.Url, req.Body);
                    break;

                case "PUT":
                    await _http.PutAsJsonAsync(req.Url, req.Body);
                    break;

                case "PATCH":
                    await _http.PatchAsJsonAsync(req.Url, req.Body);
                    break;
                case "DELETE":
                    await _http.DeleteAsync(req.Url);
                    break;

                default:
                    break;
            }
        }
    }

    public async Task ShutDown()
    {
        await SendQueuedRequests();
    }

    public async Task<Result<T?>> GetAsync<T>(string url, CancellationToken cancellationToken = default, bool saveToCache = true)
    {
        var cached = await _localStorage.GetItemAsync<Result<T>>(url);

        if (cached != null)
        {
            return cached!;
        }

        return Result.Error("Cache miss");
    }

    public Task<Result<T?>> PostAsync<T, B>(string url, B request, CancellationToken cancellationToken, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<B>
            {
                Method = HttpMethod.Post,
                BodyJson = request,
                Url = url
            });
        }
        return Task.FromResult(Result<T?>.CriticalError("Request not send"));
    }

    public Task<Result<T?>> DelAsync<T>(string url, CancellationToken cancellationToken, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<object?>
            {
                Method = HttpMethod.Delete,
                BodyJson = null,
                Url = url
            });
        }
        return Task.FromResult(Result<T?>.CriticalError("Request not send"));
    }

    public Task<Result<T?>> PutAsync<T, B>(string url, B request, CancellationToken cancellationToken, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<B>
            {
                Method = HttpMethod.Put,
                BodyJson = request,
                Url = url
            });
        }
        return Task.FromResult(Result<T?>.CriticalError("Request not send"));
    }

    public Task<Result<T?>> PatchAsync<T, B>(string url, B request, CancellationToken cancellationToken, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<B>
            {
                Method = HttpMethod.Patch,
                BodyJson = request,
                Url = url
            });
        }
        return Task.FromResult(Result<T?>.CriticalError("Request not send"));
    }

    public Task<Result> PostAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<B>
            {
                Method = HttpMethod.Post,
                BodyJson = request,
                Url = url
            });
        }
        return Task.FromResult(Result.CriticalError("Request not send"));
    }

    public Task<Result> PutAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<B>
            {
                Method = HttpMethod.Put,
                BodyJson = request,
                Url = url
            });
        }
        return Task.FromResult(Result.CriticalError("Request not send"));
    }

    public Task<Result> PostAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<object?>
            {
                Method = HttpMethod.Post,
                Url = url,
                BodyJson = null
            });
        }
        return Task.FromResult(Result.CriticalError("Request not send"));
    }

    public Task<Result> DelAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false)
    {
        if (ensureIsSend)
        {
            queuedRequests.Add(new QueuedRequest<object?>
            {
                Method = HttpMethod.Delete,
                BodyJson = null,
                Url = url
            });
        }
        return Task.FromResult(Result.CriticalError("Request not send"));
    }
}