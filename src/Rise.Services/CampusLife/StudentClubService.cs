using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.Common;

namespace Rise.Services.CampusLife;

/// <summary>
/// Service for studentclubs operations.
/// </summary>
/// <param name="dbContext"></param>
public class StudentClubService(ApplicationDbContext dbContext) : IStudentClubService
{
    public async Task<Result<StudentClubResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx)
    {
        var query = dbContext.StudentClubs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Description.Contains(request.SearchTerm) || 
                                     n.Name.Contains(request.SearchTerm));
        }
        
        var totalCount = await query.CountAsync(ctx);
        
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query
                .OrderBy(s => s.Name != "Heimdal")
                .ThenBy(s => s.Name);
        }

        
        var clubs = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(s => new StudentClubDto.Index
            {
                Name = s.Name,        
                Description = s.Description,
                WebsiteUrl = s.WebsiteUrl,
                FacebookUrl = s.FacebookUrl,
                InstagramUrl = s.InstagramUrl,
                ShieldImageUrl = s.ShieldImageUrl,
                Email = s.Email!.Value
            })
            .ToListAsync(ctx);

        return Result.Success(new StudentClubResponse.Index
        {
            StudentClubs = clubs,
            TotalCount = totalCount
        });
    }
}