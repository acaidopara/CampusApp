namespace Rise.Shared.Identity.Accounts;

/// <summary>
/// Represents the response object for account-related information.
/// This class is used to store details about user accounts such as email,
/// email confirmation status, associated claims, and roles.
/// </summary>
public static partial class AccountResponse
{
    /// <summary>
    /// Represents account information for a user.
    /// This class encapsulates details related to the user's email, email confirmation status,
    /// associated claims, and assigned roles within the system.
    /// </summary>
    public class Info
    {
        public required string Email { get; set; }
        public required bool IsEmailConfirmed { get; set; }
        public required Dictionary<string, string> Claims { get; set; } = [];
        public List<string> Roles { get; set; } = [];
    }
}