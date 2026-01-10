using Rise.Shared.Identity;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;

public class AddToUser( IShortcutService shortcutService ) 
    : Endpoint<ShortcutRequest.AddToUser, Result>
{
    public override void Configure()
    {
        Post("/api/users/{userId}/shortcuts/{shortcutId}");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(ShortcutRequest.AddToUser req, CancellationToken ct)
    {
        return await shortcutService.AddShortcutToUserAsync(req, ct);
    }
}