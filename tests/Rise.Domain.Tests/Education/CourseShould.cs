using Rise.Domain.Education;
using Ardalis.Result;

namespace Rise.Domain.Tests.Education
{
    public class CourseShould
    {
        private Deadline CreateDeadline(string title = "Homework 1") =>
            new Deadline
            {
                Title = title,
                Description = "Solve exercises",
                StartDate = DateTime.Today.AddDays(-1),
                DueDate = DateTime.Today
            };
        
        [Fact]
        public void Can_Add_Deadline()
        {
            var course = new Course { Name = "Math" };
            var deadline = CreateDeadline();

            var result = course.AddDeadline(deadline);

            result.Status.ShouldBe(ResultStatus.Ok);
            course.Deadlines.ShouldContain(deadline);
            deadline.Course.ShouldBe(course);
        }
        
        [Fact]
        public void Adding_Same_Deadline_Twice_Should_Return_Conflict()
        {
            var course = new Course { Name = "Math" };
            var deadline = CreateDeadline();

            course.AddDeadline(deadline);
            var result = course.AddDeadline(deadline);

            result.Status.ShouldBe(ResultStatus.Conflict);
        }
    }
}
