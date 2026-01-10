using Rise.Shared.Common;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;

/// <summary>
/// List all default shortcuts.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="shortcutService"></param>
public class GetDefault(IShortcutService shortcutService) 
    : Endpoint<QueryRequest.SkipTake, Result<ShortcutResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/shortcuts/default");
        AllowAnonymous(); 
    }

    public override Task<Result<ShortcutResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return shortcutService.GetDefaultShortcuts(req, ct);
    }
}