using System.Security.Claims;

namespace Rise.Shared.Identity;

/// <summary>
/// Provides extension methods for instances of the <see cref="System.Security.Claims.ClaimsPrincipal"/> class.
/// </summary>
public static class ClaimsPrincipalExtentions
{
    /// <summary>
    /// Retrieves the user ID from the given <see cref="ClaimsPrincipal"/> instance.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The user ID as a string if found; otherwise, null.</returns>
    public static string? GetUserId(this ClaimsPrincipal user) =>
        user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Retrieves the user name from the given <see cref="ClaimsPrincipal"/> instance.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The user name as a string if found; otherwise, null.</returns>
    public static string? GetUserName(this ClaimsPrincipal user) =>
        user?.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// Retrieves the email address from the given <see cref="ClaimsPrincipal"/> instance.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The email address as a string if found; otherwise, null.</returns>
    public static string? GetEmail(this ClaimsPrincipal user) =>
        user?.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Determines whether the current <see cref="ClaimsPrincipal"/> has the specified role.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <param name="role">The name of the role to check for.</param>
    /// <returns><c>true</c> if the user is in the specified role; otherwise, <c>false</c>.</returns>
    public static bool IsInRole(this ClaimsPrincipal user, string role) =>
        user?.IsInRole(role) ?? false;
}