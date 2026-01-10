namespace Rise.Shared.Identity;

/// <summary>
/// Provides a centralized definition of application roles as static properties.
/// This class is used to define role names that are utilized across the application
/// for authentication and authorization purposes.
/// </summary>
public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Student = "Student";
    public const string Teacher = "Teacher";
}