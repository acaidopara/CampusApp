using Rise.Domain.Users;
using Rise.Domain.Education;
using Ardalis.Result;

namespace Rise.Domain.Tests.Users
{
    public class StudentShould
    {
        private Departments.Department CreateDepartment() => new Departments.Department { Name = "CS", Description = "Computer Science" };
        private Student CreateStudent()
        {
            var dept = CreateDepartment();
            return new Student
            {
                Firstname = "Jane",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("jane@example.com"),
                Birthdate = new DateTime(2000,1,1),
                StudentNumber = "S12345"
            };
        }
        private ClassGroup CreateClassGroup(string name = "3A1") => new ClassGroup { Name = name };
        private Course CreateCourse(string name = "Math") => new Course { Name = name };

        [Fact]
        public void Can_Enroll_In_Course()
        {
            var student = CreateStudent();
            var course = CreateCourse();
            var classGroup = CreateClassGroup();

            var result = student.EnrollInCourse(course, classGroup);

            result.Status.ShouldBe(ResultStatus.Ok);
            student.GetCourses().ShouldContain(course);
            student.GetClassGroups().ShouldContain(classGroup);
        }

        [Fact]
        public void Enrolling_Same_Course_Twice_Should_Return_Conflict()
        {
            var student = CreateStudent();
            var course = CreateCourse();
            var classGroup = CreateClassGroup();
            
            student.EnrollInCourse(course, classGroup);
            var result = student.EnrollInCourse(course, classGroup);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
        
        [Fact]
        public void Enrolling_Null_Course_Should_Throw()
        {
            var student = CreateStudent();
            var classGroup = CreateClassGroup();
            Should.Throw<ArgumentNullException>(() => student.EnrollInCourse(null!, classGroup));
        }
        
        [Fact]
        public void Enrolling_Null_ClassGroup_Should_Throw()
        {
            var student = CreateStudent();
            var course = CreateCourse();
            Should.Throw<ArgumentNullException>(() => student.EnrollInCourse(course, null!));
        }

        [Fact]
        public void Can_Add_StudentDeadline()
        {
            var student = CreateStudent();
            var deadline = new StudentDeadline();

            var result = student.AddStudentDeadline(deadline);

            result.Status.ShouldBe(ResultStatus.Ok);
            student.StudentDeadlines.ShouldContain(deadline);
        }

        [Fact]
        public void Adding_Same_StudentDeadline_Twice_Should_Return_Conflict()
        {
            var student = CreateStudent();
            var deadline = new StudentDeadline();
            student.AddStudentDeadline(deadline);

            var result = student.AddStudentDeadline(deadline);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
        
        [Fact]
        public void Adding_Null_StudentDeadline_Should_Throw()
        {
            var student = CreateStudent();
            Should.Throw<ArgumentNullException>(() => student.AddStudentDeadline(null!));
        }
        
        [Fact]
        public void Can_Update_CampusPreference()
        {
            var student = CreateStudent();

            student.UpdateCampusPreference("Main Campus");
            student.PreferedCampus.ShouldBe("Main Campus");

            student.PreferedCampus = "Other Campus";
            student.PreferedCampus.ShouldBe("Other Campus");
        }

        [Fact]
        public void Setting_Empty_Or_Whitespace_Campus_Should_Throw()
        {
            var student = CreateStudent();
            Should.Throw<ArgumentException>(() => student.UpdateCampusPreference(""));
            Should.Throw<ArgumentException>(() => student.UpdateCampusPreference("   "));
            Should.Throw<ArgumentException>(() => student.PreferedCampus = "");
            Should.Throw<ArgumentException>(() => student.PreferedCampus = "   ");
        }

        [Fact]
        public void GetCourses_And_GetClassGroups_Returns_Distinct_Values()
        {
            var student = CreateStudent();
            var course1 = CreateCourse();
            var course2 = CreateCourse("Physics");
            var group1 = CreateClassGroup();
            var group2 = CreateClassGroup("3B1");

            student.EnrollInCourse(course1, group1);
            student.EnrollInCourse(course2, group1);
            student.EnrollInCourse(course2, group2);

            var courses = student.GetCourses();
            courses.Count.ShouldBe(2);
            courses.ShouldContain(course1);
            courses.ShouldContain(course2);

            var groups = student.GetClassGroups();
            groups.Count.ShouldBe(1);
            groups.ShouldContain(group1);
        }
    }
}
