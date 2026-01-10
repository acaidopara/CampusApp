using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class OpeningTimeShould
    {
        [Fact]
        public void Can_Create_Openingtime()
        {
            var ot = new Openingtime(
                DateOnly.FromDateTime(DateTime.Today),
                new TimeOnly(10, 0),
                new TimeOnly(14, 0)
            );

            ot.Date.ShouldBe(DateOnly.FromDateTime(DateTime.Today));
            ot.StartTime.ShouldBe(new TimeOnly(10, 0));
            ot.EndTime.ShouldBe(new TimeOnly(14, 0));
        }

        [Fact]
        public void Cannot_Create_Openingtime_With_Past_Date()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new Openingtime(
                    DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                    new TimeOnly(10, 0),
                    new TimeOnly(14, 0)
                );
            }).Message.ShouldContain("Date cannot be in the past.");
        }

        [Fact]
        public void Cannot_Create_Openingtime_With_EndTime_Earlier_Than_StartTime()
        {
            Should.Throw<ArgumentException>(() =>
            {
                new Openingtime(
                    DateOnly.FromDateTime(DateTime.Today),
                    new TimeOnly(14, 0),
                    new TimeOnly(10, 0)
                );
            }).Message.ShouldContain("StartTime must be earlier than EndTime.");
        }
    }
}