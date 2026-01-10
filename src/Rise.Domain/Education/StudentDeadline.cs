using Rise.Domain.Users;

namespace Rise.Domain.Education;

/// <summary>
/// Represents the junction entity between a Student and a Deadline.
/// This entity tracks per-student completion status for assigned deadlines,
/// allowing individual students to mark deadlines as completed without affecting others.
/// </summary>
public class StudentDeadline : Entity
{
    /// <summary>
    /// Foreign key to the associated Student.
    /// </summary>
    public int StudentId { get; set; }
    
    /// <summary>
    /// Foreign key to the associated Deadline.
    /// </summary>
    public int DeadlineId { get; set; }

    /// <summary>
    /// Indicates whether the student has completed this deadline.
    /// Defaults to false.
    /// </summary>
    public bool IsCompleted { get; set; } = false;
    
    /// <summary>
    /// The date and time when the deadline was completed, if applicable.
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Navigation property to the associated Student.
    /// </summary>
    public Student Student { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the associated Deadline.
    /// </summary>
    public Deadline Deadline { get; set; } = null!;
}