using Ardalis.Result;
using Rise.Domain.Education;

namespace Rise.Domain.Users;

public class Teacher : Employee
{
    private readonly List<Course> _courses = [];
    public IReadOnlyList<Course> Courses => _courses.AsReadOnly();

    private bool _isAbsent;
    public bool IsAbsent
    {
        get => _isAbsent;
        set => _isAbsent = value;
    }
    
    private readonly List<Lesson> _lessons = [];
    public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();
    
    public Result AddCourse(Course course)
    {
        Guard.Against.Null(course, nameof(course));

        if (_courses.Contains(course))
            return Result.Conflict("Teacher is already associated with this course.");

        _courses.Add(course);
        return Result.Success();
    }
    
    public void SyncAddLesson(Lesson lesson)
    {
        if (!_lessons.Contains(lesson))
            _lessons.Add(lesson);
    }
}