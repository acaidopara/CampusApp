using Rise.Domain.Users;

namespace Rise.Domain.Education;

public class CourseEnrollment : Entity
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public ClassGroup ClassGroup { get; set; } = null!;

    private CourseEnrollment() { }
    
    public CourseEnrollment(Course course, ClassGroup classGroup)
    {
        Course = Guard.Against.Null(course, nameof(course));
        ClassGroup = Guard.Against.Null(classGroup, nameof(classGroup));
    }
}