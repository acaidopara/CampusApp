namespace Rise.Shared.Identity;

/// <summary>
/// The AppPolicies class provides a centralized location for defining application-wide policy names.
/// These policies are used to enforce authorization rules across the application.
/// </summary>
public class AppPolicies
{
    public static string AdministratorsOnly => nameof(AdministratorsOnly);
    // Add more policies here.
}