using Rise.Shared.Identity;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;

public class RemoveFromUser( IShortcutService shortcutService )
    : Endpoint<ShortcutRequest.RemoveFromUser, Result>
{
    public override void Configure()
    {
        Delete("/api/users/{userId}/shortcuts/{shortcutId}");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(ShortcutRequest.RemoveFromUser req, CancellationToken ct)
    {
        return await shortcutService.RemoveShortcutFromUserAsync(req, ct);
    }
}