using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Deadlines;

namespace Rise.Services.Deadlines;

public class DeadlineService(ApplicationDbContext dbContext, ISessionContextProvider sessionProvider) : IDeadlineService
{
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(DeadlineRequest.GetForStudent request, CancellationToken ctx = default)
    {
        var user = sessionProvider.User;
        if (user == null || !user.Identity!.IsAuthenticated)
        {
            Log.Warning("Unauthenticated access attempt to student deadlines.");
            return Result.Unauthorized("User must be authenticated to access deadlines.");
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("User ID not found in claims for authenticated user.");
            return Result.Conflict("Unable to retrieve user ID.");
        }

        var student = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AccountId == userId, ctx);
        if (student == null)
        {
            Log.Warning("No student account linked to user ID '{UserId}'.", userId);
            return Result.NotFound("Student account not found.");
        }

        var query = dbContext.StudentDeadlines
            .Where(sd => sd.StudentId == student.Id)
            .Include(sd => sd.Deadline)
                .ThenInclude(d => d.Course)
            .Select(sd => new { sd, sd.Deadline });

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => 
                x.Deadline.Title.Contains(request.SearchTerm) || 
                x.Deadline.Description.Contains(request.SearchTerm) ||
                (x.Deadline.Course != null && x.Deadline.Course.Name.Contains(request.SearchTerm)));
        }
        
        if (!string.IsNullOrWhiteSpace(request.StartDate) && 
            DateTime.TryParse(request.StartDate, out var startDate))
        {
            query = query.Where(x => x.Deadline.DueDate.Date >= startDate.Date);
        }

        if (!string.IsNullOrWhiteSpace(request.EndDate) &&
            DateTime.TryParse(request.EndDate, out var endDate))
        {
            query = query.Where(x => x.Deadline.DueDate.Date <= endDate.Date);
        }

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            if (request.OrderBy.Equals("Course", StringComparison.OrdinalIgnoreCase))
            {
                query = request.OrderDescending
                    ? query.OrderByDescending(x => x.Deadline.Course != null ? x.Deadline.Course.Name : string.Empty)
                    : query.OrderBy(x => x.Deadline.Course != null ? x.Deadline.Course.Name : string.Empty);
            }
            else
            {
                query = request.OrderDescending
                    ? query.OrderByDescending(x => EF.Property<object>(x.Deadline, request.OrderBy))
                    : query.OrderBy(x => EF.Property<object>(x.Deadline, request.OrderBy));
            }
        }
        else
        {
            query = query.OrderBy(x => x.Deadline.DueDate);
        }

        var deadlines = await query
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new DeadlineDto.Index
            {
                Id = x.Deadline.Id,
                Title = x.Deadline.Title,
                IsCompleted = x.sd.IsCompleted,
                Description = x.Deadline.Description,
                DueDate = x.Deadline.DueDate,
                StartDate = x.Deadline.StartDate,
                Course = x.Deadline.Course != null ? x.Deadline.Course.Name : null
            })
            .ToListAsync(ctx);

        return Result.Success(new DeadlineResponse.Index
        {
            Deadlines = deadlines,
            TotalCount = totalCount
        });
    }
}