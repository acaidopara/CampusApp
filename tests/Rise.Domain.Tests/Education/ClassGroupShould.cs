using Rise.Domain.Education;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.Education
{
    public class ClassGroupShould
    {
        private ClassGroup CreateClassGroup(string name = "3A1") =>
            new ClassGroup { Name = name };

        private Student CreateStudent(string name = "Jane") =>
            new Student
            {
                Firstname = "Jane",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = new Rise.Domain.Departments.Department { Name = "CS", Description = "CS Dept" },
                Email = new EmailAddress("jane@example.com"),
                Birthdate = new DateTime(2000,1,1),
                StudentNumber = "S12345"
            };

        private Lesson CreateLesson(Course? course = null) =>
            new Lesson(
                DateTime.Today,
                DateTime.Today.AddHours(1),
                LessonType.Hoorcollege,
                course ?? new Course { Name = "Math" }
            );

        [Fact]
        public void Can_Create_ClassGroup()
        {
            var group = CreateClassGroup("Group 1");

            group.Name.ShouldBe("Group 1");
            group.Students.ShouldBeEmpty();
            group.Lessons.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Add_Student()
        {
            var group = CreateClassGroup();
            var student = CreateStudent();

            group.SyncAddStudent(student);

            group.Students.ShouldContain(student);
        }

        [Fact]
        public void Adding_Same_Student_Twice_Should_Not_Duplicate()
        {
            var group = CreateClassGroup();
            var student = CreateStudent();

            group.SyncAddStudent(student);
            group.SyncAddStudent(student);

            group.Students.Count.ShouldBe(1);
        }

        [Fact]
        public void Can_Add_Lesson()
        {
            var group = CreateClassGroup();
            var lesson = CreateLesson();

            group.SyncAddLesson(lesson);

            group.Lessons.ShouldContain(lesson);
        }

        [Fact]
        public void Adding_Same_Lesson_Twice_Should_Not_Duplicate()
        {
            var group = CreateClassGroup();
            var lesson = CreateLesson();

            group.SyncAddLesson(lesson);
            group.SyncAddLesson(lesson);

            group.Lessons.Count.ShouldBe(1);
        }
    }
}