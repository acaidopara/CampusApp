using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rise.Persistence;

namespace Rise.Server.Identity;

public class CustomClaimsPrincipalFactory(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor, ApplicationDbContext dbContext)
    : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var student = dbContext.Students.SingleOrDefaultAsync(x => x.AccountId == user.Id);
        var creationDate = student.Result?.CreatedAt.AddYears(4) ?? DateTime.Now.AddYears(4);
        
        // Add your custom claims
        identity.AddClaim(new Claim("Id", (student.Result?.Id ?? 0).ToString()));
        identity.AddClaim(new Claim("Firstname", student.Result?.Firstname ?? string.Empty));
        identity.AddClaim(new Claim("Lastname", student.Result?.Lastname ?? string.Empty));
        identity.AddClaim(new Claim("Studentnumber", student.Result?.StudentNumber ?? string.Empty));
        identity.AddClaim(new Claim("Creationdate", creationDate.ToString("dd.MM.yyyy")));
        identity.AddClaim(new Claim("Birthdate", student.Result?.Birthdate.ToString("°dd.MM.yyyy") ?? string.Empty));
        identity.AddClaim(new Claim("PreferedCampus", student.Result?.PreferedCampus ?? "Schoonmeersen"));
        identity.AddClaim(new Claim("PreferedColour", student.Result?.PreferedColour ?? "#FABC32"));

        identity.AddClaim(new Claim("LessonChanges", student.Result?.LessonChangesEnabled.ToString() ?? "false"));
        identity.AddClaim(new Claim("Absentees", student.Result?.AbsenteesEnabled.ToString() ?? "false"));
        identity.AddClaim(new Claim("NewsAndEvents", student.Result?.NewsAndEventsEnabled.ToString() ?? "false"));
        identity.AddClaim(new Claim("Deadlines", student.Result?.DeadlinesEnabled.ToString() ?? "false"));
        return identity;
    }
}