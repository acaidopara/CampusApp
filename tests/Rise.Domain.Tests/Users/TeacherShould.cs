using Ardalis.Result;
using Rise.Domain.Users;
using Rise.Domain.Education;

namespace Rise.Domain.Tests.Users
{
    public class TeacherShould
    {
        private Departments.Department CreateDepartment() => new Departments.Department { Name = "CS", Description = "Computer Science" };
        private Teacher CreateTeacher()
        {
            var dept = CreateDepartment();
            return new Teacher
            {
                Firstname = "Prof",
                Lastname = "Smith",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("prof.smith@example.com"),
                Birthdate = new DateTime(1975,1,1),
                Employeenumber = "EMP002",
                Title = "Lecturer"
            };
        }
        private Course CreateCourse(string name = "Math") => new Course { Name = name };
        private Lesson CreateLesson(Course course) => new Lesson(DateTime.Today, DateTime.Today.AddHours(1), LessonType.Hoorcollege, course);

        [Fact]
        public void Can_Add_Course_To_Teacher()
        {
            var teacher = CreateTeacher();
            var course = CreateCourse();

            var result = teacher.AddCourse(course);

            result.Status.ShouldBe(ResultStatus.Ok);
            teacher.Courses.ShouldContain(course);
        }

        [Fact]
        public void Adding_Same_Course_Twice_Should_Return_Conflict()
        {
            var teacher = CreateTeacher();
            var course = CreateCourse();

            teacher.AddCourse(course);
            var result = teacher.AddCourse(course);

            result.Status.ShouldBe(ResultStatus.Conflict);
            teacher.Courses.Count.ShouldBe(1);
        }

        [Fact]
        public void Adding_Null_Course_Should_Throw()
        {
            var teacher = CreateTeacher();

            Should.Throw<ArgumentNullException>(() => teacher.AddCourse(null!));
        }

        [Fact]
        public void Can_Add_Lesson_To_Teacher()
        {
            var teacher = CreateTeacher();
            var course = CreateCourse();
            var lesson = CreateLesson(course);

            teacher.Lessons.ShouldBeEmpty();

            teacher.SyncAddLesson(lesson);

            teacher.Lessons.ShouldContain(lesson);
            teacher.Lessons.Count.ShouldBe(1);
        }

        [Fact]
        public void Adding_Same_Lesson_Twice_Should_Not_Duplicate()
        {
            var teacher = CreateTeacher();
            var course = CreateCourse();
            var lesson = CreateLesson(course);

            teacher.SyncAddLesson(lesson);
            teacher.SyncAddLesson(lesson);

            teacher.Lessons.Count.ShouldBe(1);
            teacher.Lessons.ShouldContain(lesson);
        }
    }
}
