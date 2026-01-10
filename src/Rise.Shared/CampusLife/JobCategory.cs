namespace Rise.Shared.CampusLife;

/// <summary>
/// Represents a job category with a name and optional Font Awesome icon.
/// </summary>
public record JobCategory(string Name, string? Icon);

public static class CategoriesJob
{
    public static readonly JobCategory All = new("Alle", null);
    public static readonly JobCategory StudentJob = new("Studentenjob", "fa-solid fa-graduation-cap");
    public static readonly JobCategory Internship = new("Stage", "fa-solid fa-piggy-bank");
    public static readonly JobCategory Fulltime = new("Vast", "fa-solid fa-briefcase");

    public static readonly List<JobCategory> AllJobs = new() { All, StudentJob, Internship, Fulltime };
}