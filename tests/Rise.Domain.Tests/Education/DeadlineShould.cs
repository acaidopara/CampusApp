using Rise.Domain.Education;
using Rise.Domain.Users;
using Ardalis.Result;

namespace Rise.Domain.Tests.Education
{
    public class DeadlineShould
    {
        private Student CreateStudent() =>
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

        [Fact]
        public void Can_Assign_Student_To_Deadline()
        {
            var deadline = new Deadline { Title = "HW1", Description = "Exercises", DueDate = DateTime.Today, StartDate = DateTime.Today.AddDays(-1) };
            var student = CreateStudent();

            var result = deadline.AssignStudent(student);

            result.Status.ShouldBe(ResultStatus.Ok);
            deadline.StudentDeadlines.ShouldContain(sd => sd.StudentId == student.Id);
        }

        [Fact]
        public void Assigning_Same_Student_Twice_Should_Return_Conflict()
        {
            var deadline = new Deadline { Title = "HW1", Description = "Exercises", DueDate = DateTime.Today, StartDate = DateTime.Today.AddDays(-1) };
            var student = CreateStudent();
            deadline.AssignStudent(student);

            var result = deadline.AssignStudent(student);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
    }
}
