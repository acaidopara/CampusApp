using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Infrastructure;
using Rise.Shared.Lessons;
using Rise.Shared.Events;  // For AddressDto

namespace Rise.Services.Lessons;

public class LessonService(ApplicationDbContext dbContext, ISessionContextProvider sessionProvider) : ILessonService
{
    public async Task<Result<LessonResponse.Index>> GetIndexAsync(LessonRequest.Week request, CancellationToken ctx = default)
    {
        /* Get logged in user */
        /* ================== */
        
        var user = sessionProvider.User;
        if (user == null || !user.Identity!.IsAuthenticated)
            return Result.Unauthorized("User must be authenticated to access timetable.");

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result.Conflict("Unable to retrieve user ID.");
        
        var student = await dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.ClassGroup)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.AccountId == userId, ctx);
        
        if (student == null)
        {
            Log.Warning("No student account linked to user ID '{UserId}'.", userId);
            return Result.NotFound("Student not found.");
        }
        
        /* Check on request format */
        /* ======================= */
        if (request.StartDate > request.EndDate)
            return Result.Invalid(new ValidationError("StartDate", "Start date can't be after end date."));
        
        /* Get Lessons based on classgroups */
        /* ================================ */
        var classGroupIds = student.Enrollments.Select(e => e.ClassGroup.Id).ToList();
        var courseIds = student.Enrollments.Select(e => e.Course.Id).ToList();

        var query = dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Course)
            .Include(l => l.Classrooms).ThenInclude(c => c.Building).ThenInclude(b => b.Address)
            .Include(l => l.Classrooms).ThenInclude(c => c.Building).ThenInclude(b => b.Campus)
            .Include(l => l.Teachers)
            .Include(l => l.ClassGroups)
            .Where(l => l.ClassGroups.Any(g => classGroupIds.Contains(g.Id)) 
                        && courseIds.Contains(l.Course!.Id));
        
        if (request.StartDate != default && request.EndDate != default)
        {
            var start = request.StartDate;
            var end = request.EndDate;
            query = query.Where(l => l.StartDate.Date >= start.Date && l.StartDate.Date < end.Date);
        }
        
        var totalCount = await query.CountAsync(ctx);
        var lessonsEntities = await query
            .OrderBy(l => l.StartDate)
            .ToListAsync(ctx);
        
        var lessons = lessonsEntities
            .Select(d => new LessonDto.Index
            {
                Id = d.Id,
                Name = d.Course!.Name,
                Start = d.StartDate,
                End = d.EndDate,
                ClassroomDtos = d.Classrooms.Select(c => new ClassroomDto.Index
                {
                    Id = c.Id,
                    Number = c.Number,
                    Name = c.Name,
                    Description = c.Description,
                    Category = c.Category,
                    Floor = c.Floor,
                    Building = new BuildingDto.Index
                    {
                        Id = c.Building.Id,
                        Name = c.Building.Name,
                        Address = new AddressDto.Index
                        {
                            AddressLine1 = c.Building.Address.Addressline1,
                            AddressLine2 = c.Building.Address.Addressline2,
                            City = c.Building.Address.City,
                            PostalCode = c.Building.Address.PostalCode
                        },
                        CampusId = c.Building.Campus.Id,
                        Campus = c.Building.Campus.Name
                    }
                }).ToList(),
                LessonType = d.LessonType.ToString(),
                TeacherNames = d.Teachers.Select(t => $"{t.Firstname} {t.Lastname}").ToList(),
                ClassgroupNames = d.ClassGroups.Select(g => g.Name).ToList()
            }).ToList();
        
        return Result.Success(new LessonResponse.Index
        {
            Lessons = lessons,
            TotalCount = totalCount
        });
    }

    public async Task<Result<LessonResponse.NextLesson>> GetNextLessonAsync(CancellationToken ctx = default)
    {
        /* Get logged in user */
        /* ================== */
        
        var user = sessionProvider.User;
        if (user == null || !user.Identity!.IsAuthenticated)
            return Result.Unauthorized("User must be authenticated to access timetable.");

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result.Conflict("Unable to retrieve user ID.");
        
        var student = await dbContext.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.ClassGroup)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.AccountId == userId, ctx);
        
        if (student == null)
        {
            Log.Warning("No student account linked to user ID '{UserId}'.", userId);
            return Result.NotFound("Student not found.");
        }
        
        /* Get next lesson */
        /* =============== */
        var classGroupIds = student.Enrollments.Select(e => e.ClassGroup.Id).ToList();
        var courseIds = student.Enrollments.Select(e => e.Course.Id).ToList();
        var now = DateTime.Now;
        
        var nextLesson = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Course)
            .Include(l => l.Classrooms).ThenInclude(c => c.Building).ThenInclude(b => b.Address)
            .Include(l => l.Classrooms).ThenInclude(c => c.Building).ThenInclude(b => b.Campus)
            .Include(l => l.Teachers)
            .Include(l => l.ClassGroups)
            .Where(l => l.StartDate >= now)
            .Where(l => l.ClassGroups.Any(g => classGroupIds.Contains(g.Id)) && courseIds.Contains(l.Course!.Id))
            .OrderBy(l => l.StartDate)
            .FirstOrDefaultAsync(ctx);

        if (nextLesson == null)
            return Result.NotFound("No upcoming lessons found.");
        
        var result = new LessonDto.Index
        {
            Id = nextLesson.Id,
            Name = nextLesson.Course!.Name,
            Start = nextLesson.StartDate,
            End = nextLesson.EndDate,
            ClassroomDtos = nextLesson.Classrooms.Select(c => new ClassroomDto.Index
            {
                Id = c.Id,
                Number = c.Number,
                Name = c.Name,
                Description = c.Description,
                Category = c.Category,
                Floor = c.Floor,
                Building = new BuildingDto.Index
                {
                    Id = c.Building.Id,
                    Name = c.Building.Name,
                    Address = new AddressDto.Index
                    {
                        AddressLine1 = c.Building.Address.Addressline1,
                        AddressLine2 = c.Building.Address.Addressline2,
                        City = c.Building.Address.City,
                        PostalCode = c.Building.Address.PostalCode
                    },
                    CampusId = c.Building.Campus.Id,
                    Campus = c.Building.Campus.Name
                }
            }).ToList(),
            LessonType = nextLesson.LessonType.ToString(),
            TeacherNames = nextLesson.Teachers.Select(t => $"{t.Firstname} {t.Lastname}").ToList(),
            ClassgroupNames = nextLesson.ClassGroups.Select(g => g.Name).ToList()
        };

        return Result.Success(new LessonResponse.NextLesson
        {
            Lesson = result
        });
    }
}