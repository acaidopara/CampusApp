using Rise.Shared.Common;

namespace Rise.Shared.Shortcuts;

/// <summary>
/// Provides methods for managing shortcut-related operations.
/// </summary>
public interface IShortcutService
{
    Task<Result<ShortcutResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ct = default);

    Task<Result<ShortcutResponse.Index>> GetDefaultShortcuts(QueryRequest.SkipTake req, CancellationToken ct = default);

    Task<Result<ShortcutResponse.Index>> GetUserShortcutsAsync(ShortcutRequest.GetForUser request, CancellationToken ct = default);
    
    Task<Result> AddShortcutToUserAsync(ShortcutRequest.AddToUser request, CancellationToken ct = default);
    
    Task<Result> RemoveShortcutFromUserAsync(ShortcutRequest.RemoveFromUser request, CancellationToken ct = default);
    
    Task<Result> UpdateShortcutOrderForUserAsync(ShortcutRequest.ChangeOrder request, CancellationToken ct = default);

    Task<Result> UpdateUserShortcutColourAsync(ShortcutRequest.UpdateColour request, CancellationToken ct = default);
}
