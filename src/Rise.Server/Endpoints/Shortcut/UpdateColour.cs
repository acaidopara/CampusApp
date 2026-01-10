using Rise.Shared.Identity;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;

public class UpdateColour( IShortcutService shortcutService ) 
    : Endpoint<ShortcutRequest.UpdateColour, Result>
{
    public override void Configure()
    {
        Put("/api/users/{userId}/shortcuts/{shortcutId}/colour");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(ShortcutRequest.UpdateColour req, CancellationToken ct)
    {
        return await shortcutService.UpdateUserShortcutColourAsync(req, ct);
    }
}