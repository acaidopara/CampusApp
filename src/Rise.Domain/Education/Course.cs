using Ardalis.Result;
using Rise.Domain.Users;

namespace Rise.Domain.Education;

/// <summary>
/// Represents a course entity in the education domain.
/// A course is part of a study field and can have multiple deadlines and enrolled students.
/// This entity enforces business rules for enrollment and deadline association.
/// </summary>
public class Course : Entity
{
    public required string Name
    {
        get;
        init;
    } = Guard.Against.NullOrWhiteSpace(nameof(Name));
    
    private readonly List<Teacher> _teachers = [];
    private readonly List<CourseEnrollment> _enrollments = [];
    
    public IReadOnlyList<Teacher> Teachers => _teachers.AsReadOnly();
    public IReadOnlyList<CourseEnrollment> Enrollments => _enrollments.AsReadOnly();

    public StudyField? StudyField { get; set; }

    private List<Deadline> _deadlines = [];
    public IReadOnlyList<Deadline> Deadlines => _deadlines.AsReadOnly();

    public Result AddDeadline(Deadline deadline)
    {
        if (_deadlines.Contains(deadline))
            return Result.Conflict("Deadline already associated with this course");

        _deadlines.Add(deadline);
        deadline.Course = this;
        return Result.Success();
    }
}