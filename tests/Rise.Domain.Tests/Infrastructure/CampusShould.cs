using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class CampusShould
    {
        [Fact]
        public void Can_Create_Campus_With_Required_Fields()
        {
            var openingtimes = new List<Openingtime>
            {
                new Openingtime { Date = DateOnly.FromDateTime(DateTime.Today), StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(16,0) }
            };

            var campus = new Campus
            {
                Name = "Main Campus",
            };

            campus.Name.ShouldBe("Main Campus");
            campus.Buildings.ShouldBeEmpty();
            campus.Events.ShouldBeEmpty();
            campus.Emergencies.ShouldBeEmpty();
        }
    }
}
