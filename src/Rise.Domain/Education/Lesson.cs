using Ardalis.Result;
using Rise.Domain.Infrastructure;
using Rise.Domain.Users;

namespace Rise.Domain.Education;

public class Lesson : Entity
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    
    private readonly List<ClassGroup> _classGroups = [];
    public IReadOnlyList<ClassGroup> ClassGroups => _classGroups.AsReadOnly();
    
    private readonly List<Classroom> _classrooms = [];
    public IReadOnlyCollection<Classroom> Classrooms => _classrooms.AsReadOnly();

    private readonly List<Teacher> _teachers = [];
    public IReadOnlyCollection<Teacher> Teachers => _teachers.AsReadOnly();
    
    public LessonType LessonType { get; init; }
    public Course? Course { get; init; }
    
    private Lesson() { }
    
    public Lesson(DateTime startDate, DateTime endDate, LessonType lessonType, Course course)
    {
        Guard.Against.Null(startDate, nameof(startDate));
        Guard.Against.Null(endDate, nameof(endDate));
        Guard.Against.Null(lessonType, nameof(lessonType));
        Guard.Against.Null(course, nameof(course));
        
        if (endDate <= startDate)
            throw new ArgumentException("EndDate must be after StartDate.", nameof(endDate));
        
        StartDate = startDate;
        EndDate = endDate;
        LessonType = lessonType;
        Course = course;
    }
    
    /* Classgroup management */
    /* ===================== */
    
    public Result AddClassGroup(ClassGroup group)
    {
        Guard.Against.Null(group, nameof(group));

        if (_classGroups.Contains(group))
            return Result.Conflict("ClassGroup already assigned to this lesson.");

        _classGroups.Add(group);
        group.SyncAddLesson(this);

        return Result.Success();
    }

    /* Teacher management */
    /* ================== */
    
    public Result AddTeacher(Teacher teacher)
    {
        Guard.Against.Null(teacher, nameof(teacher));
        
        if (_teachers.Contains(teacher))
            return Result.Conflict("Teacher already assigned to lesson.");
        
        if (!teacher.Courses.Contains(Course))
            return Result.Invalid(new ValidationError("Teacher cannot teach this lesson’s course.", nameof(teacher)));
        
        _teachers.Add(teacher);
        teacher.SyncAddLesson(this);
        return Result.Success();
    }
    
    /* Classroom management */
    /* ==================== */
    
    public Result AddClassroom(Classroom classroom)
    {
        Guard.Against.Null(classroom, nameof(classroom));
        
        if (_classrooms.Contains(classroom))
            return Result.Conflict("Classroom already assigned to lesson.");

        _classrooms.Add(classroom);
        return Result.Success();
    }
}