namespace Rise.Shared.Deadlines;

/// <summary>
/// Static utility class containing data transfer object (DTO) models for deadlines.
/// These DTOs are used to transfer deadline data between layers without exposing domain entities.
/// </summary>
public static partial class DeadlineDto
{
    /// <summary>
    /// DTO for representing a deadline in index/list views.
    /// Includes essential properties for display purposes.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// The unique identifier of the deadline.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The title of the deadline.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates whether the deadline is completed by the student.
        /// </summary>
        public bool IsCompleted { get; set; }
        
        /// <summary>
        /// A description of the deadline.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// The due date for the deadline.
        /// </summary>
        public DateTime DueDate { get; set; }
        
        /// <summary>
        /// The start date for the deadline.
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// The name of the associated course, if any.
        /// </summary>
        public string? Course { get; set; }
    }
}