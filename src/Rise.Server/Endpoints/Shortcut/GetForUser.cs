using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;


public class GetForUser(IShortcutService shortcutService) 
    : Endpoint<ShortcutRequest.GetForUser, Result<ShortcutResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/users/{userId}/shortcuts");
        Roles(AppRoles.Student);
    }

    public override Task<Result<ShortcutResponse.Index>> ExecuteAsync(ShortcutRequest.GetForUser req, CancellationToken ct)
    {        
        return shortcutService.GetUserShortcutsAsync(req, ct);
    }
}