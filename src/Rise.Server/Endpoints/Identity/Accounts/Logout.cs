using Microsoft.AspNetCore.Identity;

namespace Rise.Server.Endpoints.Identity.Accounts;

/// <summary>
/// Logout Endpoint.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="signInManager"></param>
public class Logout(SignInManager<IdentityUser> signInManager) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/identity/accounts/logout");
    }

    public override async Task<Result> HandleAsync(CancellationToken ct)
    {
        await signInManager.SignOutAsync();
        return Result.NoContent();       
    }
}