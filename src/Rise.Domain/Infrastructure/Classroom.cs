using Ardalis.Result;
using Rise.Domain.Education;

namespace Rise.Domain.Infrastructure;

/// <summary>
/// Represents a classroom entity in the infrastructure domain.
/// A classroom is a physical space within a building used for educational purposes,
/// associated with lessons. This entity enforces business rules for property validation
/// and lesson management.
/// </summary>
public class Classroom : Entity
{
    /// <summary>
    /// The number of the classroom, required and non-empty.
    /// </summary>
    private string _number = string.Empty;
    public string Number
    {
        get => _number;
        set => _number = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The name of the classroom, required and non-empty.
    /// </summary>
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// A description of the classroom, required and non-empty.
    /// </summary>
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The category of the classroom, required and non-empty (e.g., lecture hall, lab).
    /// </summary>
    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set => _category = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The floor where the classroom is located, required and non-empty.
    /// </summary>
    private string _floor = string.Empty;
    public string Floor
    {
        get => _floor;
        set => _floor = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The building associated with this classroom, required.
    /// </summary>
    private Building _building = null!;
    public Building Building
    {
        get => _building;
        set => _building = Guard.Against.Null(value);
    }

    // Derived properties (no direct Campus field; access via Building.Campus)
    public Campus Campus => Building.Campus;
    public string CampusName => Campus.Name;

    /// <summary>
    /// Backing field for the list of lessons in this classroom.
    /// </summary>
    private readonly List<Lesson> _lessons = [];

    /// <summary>
    /// Read-only collection of lessons associated with this classroom.
    /// </summary>
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    /// <summary>
    /// Adds a lesson to this classroom, ensuring it is not already added
    /// and maintains bidirectional relationships.
    /// </summary>
    /// <param name="lesson">The lesson to add.</param>
    /// <returns>A Result indicating success or conflict if already added.</returns>
    public Result AddLesson(Lesson lesson)
    {
        Guard.Against.Null(lesson);

        if (_lessons.Any(l => l.Id == lesson.Id))
            return Result.Conflict("Lesson already added to this classroom");

        _lessons.Add(lesson);
        return Result.Success();
    }
}