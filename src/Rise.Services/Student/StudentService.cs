using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Student;

namespace Rise.Services.Student;

/// <summary>
/// Service for users.
/// </summary>
/// <param name="dbContext"></param>
public class StudentService(ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : IStudentService
{
    public async Task<Result> UpdateCampusPreference(
        StudentRequest.Campus request,
        CancellationToken ct = default)
    {
        var student = await dbContext.Students
            .Where(u => u.Id == request.UserId)
            .FirstOrDefaultAsync(ct);

        if (ReferenceEquals(student, null))
            return Result.Invalid(new ValidationError("Student not found."));
        
        student.UpdateCampusPreference(request.CampusName);
        await dbContext.SaveChangesAsync(ct);

        var user = await userManager.FindByIdAsync(student.AccountId);
        if (user is not null)
            await signInManager.RefreshSignInAsync(user);

        return Result.Success();
    }
    
    public async Task<Result> UpdateColourPreference(
        StudentRequest.Colour request,
        CancellationToken ct = default)
    {
        var student = await dbContext.Students
            .Where(u => u.Id == request.UserId)
            .FirstOrDefaultAsync(ct);

        if (ReferenceEquals(student, null))
            return Result.Invalid(new ValidationError("Student not found."));
        
        student.UpdateColourPreference(request.ColourHex);
        await dbContext.SaveChangesAsync(ct);

        var user = await userManager.FindByIdAsync(student.AccountId);
        if (user is not null)
            await signInManager.RefreshSignInAsync(user);

        return Result.Success();
    }

    public async Task<Result> UpdatePushNotifPreferences(
        StudentRequest.Preference request,
        CancellationToken ct = default)
    {
        var student = await dbContext.Students
            .Where(u => u.Id == request.UserId)
            .FirstOrDefaultAsync(ct);

        if (ReferenceEquals(student, null))
            return Result.Invalid(new ValidationError("Student not found."));
        
        student.UpdateNotificationPreferences(
            request.LessonChangesEnabled,
            request.AbsenteesEnabled,
            request.NewsAndEventsEnabled,
            request.DeadlinesEnabled
        );
        await dbContext.SaveChangesAsync(ct);

        var user = await userManager.FindByIdAsync(student.AccountId);
        if (user is not null)
            await signInManager.RefreshSignInAsync(user);

        return Result.Success();
    }
}