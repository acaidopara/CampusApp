using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class BuildingShould
    {
        private Campus CreateCampus() => new Campus ();
        private List<Openingtime> CreateOpeningTimes() => new List<Openingtime> { new Openingtime { Date = DateOnly.FromDateTime(DateTime.Today), StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(16,0) } };

        [Fact]
        public void Can_Create_Building_With_Required_Fields()
        {
            var campus = CreateCampus();
            var openingtimes = CreateOpeningTimes();

            var building = new Building
            {
                Name = "Building A",
                Campus = campus,
            };

            building.Name.ShouldBe("Building A");
            building.Campus.ShouldBe(campus);
            building.Classrooms.ShouldBeEmpty();
            building.Restos.ShouldBeEmpty();
        }

        [Fact]
        public void Setting_Name_To_NullOrWhitespace_Should_Throw()
        {
            var building = new Building()
            {
                Campus = CreateCampus(),
            };
            Should.Throw<ArgumentException>(() => building.Name = null!);
            Should.Throw<ArgumentException>(() => building.Name = "");
            Should.Throw<ArgumentException>(() => building.Name = "   ");
        }
    }
}
