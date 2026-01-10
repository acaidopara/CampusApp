using System.Security.Claims;
using Rise.Services.Identity;

namespace Rise.Services.Tests.Fakers;

public class FakeSessionContextProvider : ISessionContextProvider
{
    public FakeSessionContextProvider(ClaimsPrincipal user) => User = user;
    public ClaimsPrincipal? User { get; }
    public string? Id { get; }
    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Studentnumber { get; }
    public string? Creationdate { get; }
    public string? PreferedCampus { get; }
}