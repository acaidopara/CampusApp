using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Shortcuts;
using Rise.Client.Api;

namespace Rise.Client.Pages.Shortcuts.Service;

public class ShortcutService(TransportProvider _transport) : IShortcutService
{
    public async Task<Result<ShortcutResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await _transport.Current.GetAsync<ShortcutResponse.Index>("/api/shortcuts", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result<ShortcutResponse.Index>> GetDefaultShortcuts(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await _transport.Current.GetAsync<ShortcutResponse.Index>("/api/shortcuts/default", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result<ShortcutResponse.Index>> GetUserShortcutsAsync(ShortcutRequest.GetForUser request, CancellationToken ctx = default)
    {
        var result = await _transport.Current.GetAsync<ShortcutResponse.Index>($"/api/users/{request.UserId}/shortcuts", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result> AddShortcutToUserAsync(ShortcutRequest.AddToUser request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PostAsync<ShortcutRequest.AddToUser>(
            $"/api/users/{request.UserId}/shortcuts/{request.ShortcutId}", request, ctx
        ))!;
    }

    public async Task<Result> RemoveShortcutFromUserAsync(ShortcutRequest.RemoveFromUser request, CancellationToken ctx = default)
    {
       return (await _transport.Current.DelAsync($"/api/users/{request.UserId}/shortcuts/{request.ShortcutId}", ctx))!;
       
    }

    public async Task<Result> UpdateShortcutOrderForUserAsync(ShortcutRequest.ChangeOrder request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PutAsync<ShortcutRequest.ChangeOrder>($"/api/users/{request.UserId}/shortcuts/order", request, ctx))!;
    }
    public async Task<Result> UpdateUserShortcutColourAsync(ShortcutRequest.UpdateColour request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PutAsync<ShortcutRequest.UpdateColour>($"/api/users/{request.UserId}/shortcuts/{request.ShortcutId}/colour", request, ctx))!;
    }
}