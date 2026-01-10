using Rise.Domain.Education;
using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class ClassroomShould
    {
        private Building CreateBuilding() => new Building { Name = "Building A", Campus = new Campus () };

        [Fact]
        public void Can_Create_Classroom_With_Required_Fields()
        {
            var building = CreateBuilding();

            var classroom = new Classroom
            {
                Building = building
            };

            classroom.Building.ShouldBe(building);
            classroom.Lessons.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Add_Lesson()
        {
            var classroom = new Classroom { Building = CreateBuilding() };
            var lesson = new Rise.Domain.Education.Lesson(DateTime.Now, DateTime.Now.AddHours(1), LessonType.Hoorcollege, new Course(){Name = "RISE"});

            classroom.AddLesson(lesson);

            classroom.Lessons.Count.ShouldBe(1);
            classroom.Lessons.ShouldContain(lesson);
        }
    }
}
