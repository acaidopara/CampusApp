using Ardalis.Result;
using Rise.Domain.Education;

namespace Rise.Domain.Users;

public class Student : User
{
    private readonly List<CourseEnrollment> _enrollments = [];
    private List<StudentDeadline> _studentDeadlines = [];
    private string _preferedCampus = string.Empty;
    
    public IReadOnlyList<CourseEnrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyList<StudentDeadline> StudentDeadlines => _studentDeadlines.AsReadOnly();
    
    public required string StudentNumber { get; init; } = Guard.Against.NullOrEmpty(nameof(StudentNumber));
    
    public string PreferedCampus
    {
        get => _preferedCampus;
        set => _preferedCampus = Guard.Against.NullOrWhiteSpace(value, nameof(PreferedCampus));
    }
    
    public void UpdateCampusPreference(string campus) => PreferedCampus = campus;

    public Result EnrollInCourse(Course course, ClassGroup classGroup)
    {
        Guard.Against.Null(course, nameof(course));
        Guard.Against.Null(classGroup, nameof(classGroup));

        if (_enrollments.Any(e => e.Course == course))
            return Result.Conflict("Student is already enrolled in this course.");

        if (_enrollments.Any(e => e.ClassGroup == classGroup && e.Course == course))
            return Result.Conflict("Student is already assigned to this class group for this course.");

        _enrollments.Add(new CourseEnrollment(course, classGroup));
        classGroup.SyncAddStudent(this);
        return Result.Success();
    }

    private string _preferedColour = string.Empty;

    public string PreferedColour
    {
        get => _preferedColour;
        set => _preferedColour = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private bool _lessonChangesEnabled;
    public bool LessonChangesEnabled
    {
        get => _lessonChangesEnabled;
        set => _lessonChangesEnabled = value;
    }

    private bool _absenteesEnabled;
    public bool AbsenteesEnabled
    {
        get => _absenteesEnabled;
        set => _absenteesEnabled = value;
    }

    private bool _newsAndEventsEnabled;
    public bool NewsAndEventsEnabled
    {
        get => _newsAndEventsEnabled;
        set => _newsAndEventsEnabled = value;
    }

    private bool _deadlinesEnabled;
    public bool DeadlinesEnabled
    {
        get => _deadlinesEnabled;
        set => _deadlinesEnabled = value;
    }
    
    public void UpdateNotificationPreferences(
        bool lessonChanges,
        bool absentees,
        bool newsAndEvents,
        bool deadlines)
    {
        LessonChangesEnabled = lessonChanges;
        AbsenteesEnabled = absentees;
        NewsAndEventsEnabled = newsAndEvents;
        DeadlinesEnabled = deadlines;
    }
    
    public void UpdateColourPreference(string colour)
    {
        PreferedColour = colour;
    }
    
    public IReadOnlyList<Course> GetCourses() => _enrollments.Select(e => e.Course).ToList().AsReadOnly();
    public IReadOnlyList<ClassGroup> GetClassGroups() => _enrollments.Select(e => e.ClassGroup).Distinct().ToList().AsReadOnly();

    public Result AddStudentDeadline(StudentDeadline studentDeadline)
    {
        Guard.Against.Null(studentDeadline, nameof(studentDeadline));
        
        if (_studentDeadlines.Contains(studentDeadline))
            return Result.Conflict("Deadline already assigned to this student");

        _studentDeadlines.Add(studentDeadline);
        return Result.Success();
    }
}