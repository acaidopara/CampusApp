using System.Security.Claims;
using Rise.Services.Identity;

namespace Rise.Server.Identity;

/// <summary>
/// Provides the current user from the HttpContext to the session provider.
/// Basically so you can use the session provider in the Service layer.
/// <see cref="ISessionContextProvider"/>
/// </summary>
/// <param name="httpContextAccessor"></param>
public class HttpContextSessionProvider(IHttpContextAccessor httpContextAccessor) : ISessionContextProvider 
{ 
    public ClaimsPrincipal? User => httpContextAccessor!.HttpContext?.User;

    public string? Id
    {
        get
        {
            var value = User?.FindFirst("Id")?.Value;
            return value;
        }
    }
    
    public string? Firstname
    {
        get
        {
            var value = User?.FindFirst("Firstname")?.Value;
            return value;
        }
    }
    
    public string? Lastname
    {
        get
        {
            var value = User?.FindFirst("Lastname")?.Value;
            return value;
        }
    }
    
    public string? Studentnumber
    {
        get
        {
            var value = User?.FindFirst("Studentnumber")?.Value;
            return value;
        }
    }
    
    public string? Creationdate
    {
        get
        {
            var value = User?.FindFirst("Creationdate")?.Value;
            return value;
        }
    }
    
    public string? Birthdate
    {
        get
        {
            var value = User?.FindFirst("Birthdate")?.Value;
            return value;
        }
    }
    
    public string? PreferedCampus
    {
        get
        {
            var value = User?.FindFirst("PreferedCampus")?.Value;
            return value;
        }
    }
    
    public string? LessonChanges
    {
        get
        {
            var value = User?.FindFirst("LessonChanges")?.Value;
            return value;
        }
    }
    
    public string? Absentees
    {
        get
        {
            var value = User?.FindFirst("Absentees")?.Value;
            return value;
        }
    }
    
    public string? NewsAndEvents
    {
        get
        {
            var value = User?.FindFirst("NewsAndEvents")?.Value;
            return value;
        }
    }
    
    public string? Deadlines
    {
        get
        {
            var value = User?.FindFirst("Deadlines")?.Value;
            return value;
        }
    }
    
    public string? PreferedColour
    {
        get
        {
            var value = User?.FindFirst("PreferedColour")?.Value;
            return value;
        }
    }
} 
