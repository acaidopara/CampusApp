namespace Rise.Shared.News;

/// <summary>
/// Represents a news topic with a name and optional Font Awesome icon.
/// </summary>
public record NewsTopic(string Name, string? Icon);

public static class Topics
{
    public static readonly NewsTopic All = new("Alle", null);
    public static readonly NewsTopic WellBeing = new("Welzijn", "fa-solid fa-heart");
    public static readonly NewsTopic Sports = new("Sport", "fa-solid fa-basketball");
    public static readonly NewsTopic Education = new("Onderwijs", "fa-solid fa-graduation-cap");
    public static readonly NewsTopic StudentAssociation = new("Studentenvereniging", "fa-solid fa-users");

    public static readonly List<NewsTopic> AllTopics = new() { All, WellBeing, Sports, Education, StudentAssociation };
}