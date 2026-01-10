using Ardalis.Result;
using Ardalis.GuardClauses;
using Rise.Domain.Users;

namespace Rise.Domain.Education;

/// <summary>
/// Represents a deadline entity in the education domain.
/// A deadline is a time-bound task or assignment associated with a course,
/// which can be assigned to multiple students via the StudentDeadline junction.
/// This entity enforces business rules for assignment and completion tracking.
/// </summary>
public class Deadline : Entity
{
    /// <summary>
    /// The title of the deadline, required and non-empty.
    /// </summary>
    private string _title = string.Empty;
    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    
    /// <summary>
    /// A description of the deadline, required and non-empty.
    /// </summary>
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The due date for the deadline.
    /// </summary>
    private DateTime _dueDate;
    public DateTime DueDate
    {
        get => _dueDate;
        set => _dueDate = value; 
    }

    /// <summary>
    /// The start date for the deadline.
    /// </summary>
    private DateTime _startDate;
    public DateTime StartDate
    {
        get => _startDate;
        set => _startDate = value; 
    }

    /// <summary>
    /// The optional associated course for this deadline.
    /// </summary>
    public Course? Course { get; set; }

    /// <summary>
    /// Backing field for the list of student assignments to this deadline.
    /// </summary>
    private List<StudentDeadline> studentDeadlines = [];
    
    /// <summary>
    /// Read-only list of student assignments (junction entities) for this deadline.
    /// </summary>
    public IReadOnlyList<StudentDeadline> StudentDeadlines => studentDeadlines.AsReadOnly();

    /// <summary>
    /// Assigns a student to this deadline, creating a new StudentDeadline junction entity.
    /// Ensures the student is not already assigned and maintains bidirectional relationships.
    /// </summary>
    /// <param name="student">The student to assign.</param>
    /// <returns>A Result indicating success or conflict if already assigned.</returns>
    public Result AssignStudent(Student student)
    {
        if (studentDeadlines.Any(sd => sd.StudentId == student.Id))
            return Result.Conflict("Student already assigned to this deadline");

        var studentDeadline = new StudentDeadline
        {
            Student = student,
            StudentId = student.Id,
            Deadline = this,
            DeadlineId = this.Id,
            IsCompleted = false
        };

        studentDeadlines.Add(studentDeadline);
        student.AddStudentDeadline(studentDeadline); // Make the relationship bidirectional
        return Result.Success();
    }
}