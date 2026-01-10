using Ardalis.Result;
using Rise.Domain.Education;
using Rise.Domain.Users;
using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Education
{
    public class LessonShould
    {
        private Course CreateCourse() => new Course { Name = "Math" };
        
        private Teacher CreateTeacher(Course? course = null)
        {
            var dept = new Departments.Department { Name = "CS", Description = "CS Dept" };
            var teacher = new Teacher
            {
                Firstname = "John",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("john.doe@example.com"),
                Birthdate = new DateTime(1980, 1, 1),
            };

            if (course != null)
                teacher.AddCourse(course);

            return teacher;
        }
        
        private Classroom CreateClassroom(string buildingName = "B1")
        {
            return new Classroom { Building = new Building { Name = buildingName, Campus = new Campus() } };
        }

        private ClassGroup CreateClassGroup(string name = "3A1") => new ClassGroup { Name = name };

        [Fact]
        public void Can_Create_Lesson()
        {
            var course = CreateCourse();
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, course);

            lesson.StartDate.ShouldBe(DateTime.Today);
            lesson.EndDate.ShouldBe(DateTime.Today.AddHours(2));
            lesson.LessonType.ShouldBe(LessonType.Hoorcollege);
            lesson.Course.ShouldBe(course);
            lesson.Classrooms.ShouldBeEmpty();
            lesson.Teachers.ShouldBeEmpty();
            lesson.ClassGroups.ShouldBeEmpty();
        }
        
        [Fact]
        public void Constructor_Should_Throw_When_EndDate_Before_StartDate()
        {
            var course = CreateCourse();

            Should.Throw<ArgumentException>(() =>
                new Lesson(DateTime.Today.AddHours(1), DateTime.Today, LessonType.Hoorcollege, course));
        }
        
        [Fact]
        public void Can_Add_ClassGroup()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            var group = CreateClassGroup();

            var result = lesson.AddClassGroup(group);

            result.Status.ShouldBe(ResultStatus.Ok);
            lesson.ClassGroups.ShouldContain(group);
            group.Lessons.ShouldContain(lesson);
        }
        
        [Fact]
        public void Adding_Same_ClassGroup_Twice_Should_Return_Conflict()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            var group = CreateClassGroup();
            lesson.AddClassGroup(group);

            var result = lesson.AddClassGroup(group);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
        
        [Fact]
        public void Can_Add_Teacher()
        {
            var course = CreateCourse();
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, course);
            var teacher = CreateTeacher(course);

            var result = lesson.AddTeacher(teacher);

            result.Status.ShouldBe(ResultStatus.Ok);
            lesson.Teachers.ShouldContain(teacher);
        }
        
        [Fact]
        public void Adding_Teacher_Twice_Should_Return_Conflict()
        {
            var course = CreateCourse();
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, course);
            var teacher = CreateTeacher(course);

            lesson.AddTeacher(teacher);
            var result = lesson.AddTeacher(teacher);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
        
        [Fact]
        public void Adding_Teacher_Not_Teaching_Course_Should_Return_Invalid()
        {
            var course = CreateCourse();
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, course);
            var teacher = CreateTeacher();

            var result = lesson.AddTeacher(teacher);

            result.Status.ShouldBe(ResultStatus.Invalid);
        }

        [Fact]
        public void Can_Add_Classroom()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            var classroom = CreateClassroom();

            var result = lesson.AddClassroom(classroom);

            result.Status.ShouldBe(ResultStatus.Ok);
            lesson.Classrooms.ShouldContain(classroom);
        }
        
        [Fact]
        public void Adding_Same_Classroom_Twice_Should_Return_Conflict()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            var classroom = CreateClassroom();

            lesson.AddClassroom(classroom);
            var result = lesson.AddClassroom(classroom);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
        
        [Fact]
        public void Add_Null_ClassGroup_Should_Throw()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            Should.Throw<ArgumentNullException>(() => lesson.AddClassGroup(null!));
        }

        [Fact]
        public void Add_Null_Teacher_Should_Throw()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            Should.Throw<ArgumentNullException>(() => lesson.AddTeacher(null!));
        }

        [Fact]
        public void Add_Null_Classroom_Should_Throw()
        {
            var lesson = new Lesson(DateTime.Today, DateTime.Today.AddHours(2), LessonType.Hoorcollege, CreateCourse());
            Should.Throw<ArgumentNullException>(() => lesson.AddClassroom(null!));
        }
    }
}
