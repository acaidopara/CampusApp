using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Absences;

namespace Rise.Services.Absences;

public class AbsenceService(ApplicationDbContext dbContext) : IAbsenceService
{
    public async Task<Result<AbsenceResponse.Index>> GetAbsencesForDay(
        AbsenceRequest.DayRequest request, 
        CancellationToken ctx = default)
    {
        var query = dbContext.Teachers
            .AsNoTracking()
            .Where(t => t.IsAbsent);

        var totalCount = await query.CountAsync(ctx);

        var absences = await query
            .Select(t => new AbsenceDto.Index
            {
                Id = t.Id,
                DateAndTime = request.Day,
                TeacherName = t.Firstname + " " + t.Lastname
            })
            .ToListAsync(ctx);

        return Result.Success(new AbsenceResponse.Index
        {
            Absences = absences,
            TotalCount = totalCount
        });
    }
}