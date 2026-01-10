using Rise.Domain.Education;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.Education
{
    public class CourseEnrollmentShould
    {
        private Course CreateCourse(string name = "Math") => new Course { Name = name };
        private ClassGroup CreateClassGroup(string name = "3A1") => new ClassGroup { Name = name };
        private Student CreateStudent(string name = "Jane") =>
            new Student
            {
                Firstname = name,
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = new Departments.Department { Name = "CS", Description = "CS Dept" },
                Email = new EmailAddress("jane@example.com"),
                Birthdate = new DateTime(2000, 1, 1),
                StudentNumber = "S12345"
            };

        [Fact]
        public void Can_Create_CourseEnrollment_With_Course_And_ClassGroup()
        {
            var course = CreateCourse();
            var group = CreateClassGroup();

            var enrollment = new CourseEnrollment(course, group);

            enrollment.Course.ShouldBe(course);
            enrollment.ClassGroup.ShouldBe(group);
        }

        [Fact]
        public void Can_Assign_Student_After_Creation()
        {
            var course = CreateCourse();
            var group = CreateClassGroup();
            var student = CreateStudent();

            var enrollment = new CourseEnrollment(course, group)
            {
                Student = student
            };

            enrollment.Student.ShouldBe(student);
        }
    }
}