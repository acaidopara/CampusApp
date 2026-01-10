using Rise.Shared.Identity;
using Rise.Shared.Shortcuts;

namespace Rise.Server.Endpoints.Shortcut;

public class ChangeOrder( IShortcutService shortcutService )
    : Endpoint<ShortcutRequest.ChangeOrder, Result>
{
    public override void Configure()
    {
        Put("/api/users/{UserId}/shortcuts/order");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(ShortcutRequest.ChangeOrder req, CancellationToken ct)
    {
        return await shortcutService.UpdateShortcutOrderForUserAsync(req, ct);
    }
}