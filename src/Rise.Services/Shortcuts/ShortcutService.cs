using Microsoft.EntityFrameworkCore;
using Rise.Domain.Dashboard;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Shortcuts;

namespace Rise.Services.Shortcuts;

/// <summary>
/// Service for shortcuts.
/// </summary>
/// <param name="dbContext"></param>
public class ShortcutService(ApplicationDbContext dbContext) : IShortcutService
{
    public async Task<Result<ShortcutResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx)
    {
        var query = dbContext.Shortcuts.AsQueryable();

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(s => s.Title);
        }

        var shortcuts = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(s => new ShortcutDto.Index
            {
                Id = s.Id,
                Title = s.Title,
                Icon = s.Icon,
                Label = s.Label,
                LinkUrl = s.LinkUrl,
                ShortcutType = s.ShortcutType
            })
            .ToListAsync(ctx);

        return Result.Success(new ShortcutResponse.Index
        {
            Shortcuts = shortcuts,
            TotalCount = totalCount
        });
    }
    
    public async Task<Result<ShortcutResponse.Index>> GetDefaultShortcuts(QueryRequest.SkipTake request,
        CancellationToken ctx)
    {
        var query = dbContext.Shortcuts.AsQueryable();

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(s => s.Title);
        }

        var shortcuts = await query.AsNoTracking()
            .Where(s => s.DefaultForGuest)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(s => new ShortcutDto.Index
            {
                Id = s.Id,
                Title = s.Title,
                Icon = s.Icon,
                Label = s.Label,
                LinkUrl = s.LinkUrl,
                ShortcutType = s.ShortcutType
            })
            .ToListAsync(ctx);

        return Result.Success(new ShortcutResponse.Index
        {
            Shortcuts = shortcuts,
            TotalCount = totalCount
        });
    }
    
    public async Task<Result<ShortcutResponse.Index>> GetUserShortcutsAsync(
        ShortcutRequest.GetForUser request,
        CancellationToken ctx = default)
    {
        var query = dbContext.UserShortcuts
            .Include(us => us.Shortcut)
            .Where(us => us.UserId == request.UserId)
            .Where(us => us.IsDeleted == false)
            .AsQueryable();

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(us => us.Position);
        }

        var shortcuts = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(us => new ShortcutDto.Index
            {
                Id = us.Shortcut!.Id,
                Title = us.Shortcut.Title,
                Icon = us.Shortcut.Icon,
                Label = us.Shortcut.Label,
                LinkUrl = us.Shortcut.LinkUrl,
                ShortcutType = us.Shortcut.ShortcutType,
                Colour = us.Colour!
            })
            .ToListAsync(ctx);
        
        return Result.Success(new ShortcutResponse.Index
        {
            Shortcuts = shortcuts,
            TotalCount = totalCount
        });
    }
    
    public async Task<Result> AddShortcutToUserAsync(
        ShortcutRequest.AddToUser request,
        CancellationToken ct = default)
    {
        var shortcutExists = await dbContext.Shortcuts
            .AnyAsync(s => s.Id == request.ShortcutId, ct);

        if (!shortcutExists)
            return Result.NotFound($"Shortcut with ID {request.ShortcutId} not found.");

        var existingUserShortcut = await dbContext.UserShortcuts
            .FirstOrDefaultAsync(us =>
                    us.UserId == request.UserId &&
                    us.ShortcutId == request.ShortcutId,
                ct);

        if (existingUserShortcut is not null && !existingUserShortcut.IsDeleted)
            return Result.Invalid(new ValidationError("This shortcut is already added to this user."));

        if (existingUserShortcut is not null && existingUserShortcut.IsDeleted)
        {
            existingUserShortcut.IsDeleted = false;
            await dbContext.SaveChangesAsync(ct);
            return Result.Success();
        }

        var maxPosition = await dbContext.UserShortcuts
            .Where(us => us.UserId == request.UserId)
            .MaxAsync(us => (int?)us.Position, ct) ?? -1;

        var userShortcut = new UserShortcut(
            request.UserId,
            request.ShortcutId,
            maxPosition + 1
        );

        dbContext.UserShortcuts.Add(userShortcut);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }


    public async Task<Result> RemoveShortcutFromUserAsync(
        ShortcutRequest.RemoveFromUser request,
        CancellationToken ct = default)
    {
        var userShortcut = await dbContext.UserShortcuts
            .FirstOrDefaultAsync(us =>
                    us.UserId == request.UserId &&
                    us.ShortcutId == request.ShortcutId,
                ct);

        if (userShortcut is null)
            return Result.NotFound("Shortcut not found for this user.");

        if (userShortcut.IsDeleted)
            return Result.Invalid(new ValidationError("Shortcut is already removed."));

        userShortcut.IsDeleted = true;

        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }


    public async Task<Result> UpdateShortcutOrderForUserAsync(
        ShortcutRequest.ChangeOrder request,
        CancellationToken ct = default)
    {
        var userShortcuts = await dbContext.UserShortcuts
            .Where(us => us.UserId == request.UserId)
            .ToListAsync(ct);

        if (userShortcuts.Count == 0)
            return Result.NotFound("No shortcuts found for this user.");

        var invalidIds = request.OrderedShortcutIds
            .Where(id => userShortcuts.All(us => us.ShortcutId != id))
            .ToList();

        if (invalidIds.Count != 0)
            return Result.Invalid(new ValidationError("One or more shortcut IDs are invalid."));

        for (var i = 0; i < request.OrderedShortcutIds.Count; i++)
        {
            var shortcut = userShortcuts.First(us => us.ShortcutId == request.OrderedShortcutIds[i]);
            shortcut.UpdatePosition(i);
        }

        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> UpdateUserShortcutColourAsync(ShortcutRequest.UpdateColour request,
        CancellationToken ct = default)
    {
        var userShortcut = await dbContext.UserShortcuts
            .FirstOrDefaultAsync(us =>
                    us.UserId == request.UserId &&
                    us.ShortcutId == request.ShortcutId,
                ct);

        if (userShortcut is null)
            return Result.NotFound("Shortcut not found for this user.");

        userShortcut.UpdateColour(request.Colour);

        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}