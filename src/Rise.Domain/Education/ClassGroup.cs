using Rise.Domain.Users;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Rise.Domain.Tests")]
[assembly: InternalsVisibleTo("Rise.Services.Tests")]

namespace Rise.Domain.Education
{
    public class ClassGroup : Entity
    {
        private readonly List<Student> _students = [];
        private readonly List<Lesson> _lessons = [];

        public required string Name { get; init; } = Guard.Against.NullOrEmpty(nameof(Name));

        public IReadOnlyList<Student> Students => _students.AsReadOnly();
        public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();

        internal void SyncAddStudent(Student student)
        {
            if (!_students.Contains(student))
                _students.Add(student);
        }

        internal void SyncAddLesson(Lesson lesson)
        {
            if (!_lessons.Contains(lesson))
                _lessons.Add(lesson);
        }
    }
}