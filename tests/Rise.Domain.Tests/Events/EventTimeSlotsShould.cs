using Rise.Domain.Events;

namespace Rise.Domain.Tests.Events
{
    public class EventTimeSlotsShould
    {
        [Fact]
        public void Can_Create_EventTimeSlot_With_Valid_Date_And_Time()
        {
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var startTime = new TimeOnly(10, 0);
            var endTime = new TimeOnly(12, 0);

            var slot = new EventTimeSlot(date, startTime, endTime);

            slot.Date.ShouldBe(date);
            slot.StartTime.ShouldBe(startTime);
            slot.EndTime.ShouldBe(endTime);
        }


        [Fact]
        public void Can_Create_EventTimeSlot_Without_EndTime()
        {
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var startTime = new TimeOnly(14, 30);

            var slot = new EventTimeSlot(date, startTime);

            slot.Date.ShouldBe(date);
            slot.StartTime.ShouldBe(startTime);
            slot.EndTime.ShouldBeNull();
        }

        [Fact]
        public void Creating_EventTimeSlot_With_EndTime_Before_StartTime_Should_Throw()
        {
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var startTime = new TimeOnly(15, 0);
            var endTime = new TimeOnly(14, 0);

            var ex = Should.Throw<ArgumentException>(() => new EventTimeSlot(date, startTime, endTime));

            ex.Message.ShouldContain("EndTime must be greater than or equal to StartTime");
        }

        [Fact]
        public void Creating_EventTimeSlot_With_Past_Date_Should_Throw()
        {
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var startTime = new TimeOnly(10, 0);

            var ex = Should.Throw<ArgumentOutOfRangeException>(() => new EventTimeSlot(pastDate, startTime));

            ex.Message.ShouldContain("Date must be in the future");
        }

    }
}