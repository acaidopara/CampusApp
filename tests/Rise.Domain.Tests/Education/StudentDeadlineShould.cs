using Rise.Domain.Education;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.Education
{
    public class StudentDeadlineShould
    {
        [Fact]
        public void Can_Create_StudentDeadline()
        {
            var student = new Student
            {
                Firstname = "Jane",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = new Rise.Domain.Departments.Department { Name = "CS", Description = "CS Dept" },
                Email = new EmailAddress("jane@example.com"),
                Birthdate = new DateTime(2000,1,1),
                StudentNumber = "S12345"
            };

            var deadline = new Deadline { Title = "HW1", Description = "Exercises", DueDate = DateTime.Today, StartDate = DateTime.Today.AddDays(-1) };

            var sd = new StudentDeadline
            {
                Student = student,
                StudentId = student.Id,
                Deadline = deadline,
                DeadlineId = deadline.Id,
                IsCompleted = false
            };

            sd.Student.ShouldBe(student);
            sd.Deadline.ShouldBe(deadline);
            sd.IsCompleted.ShouldBeFalse();
        }
    }
}
