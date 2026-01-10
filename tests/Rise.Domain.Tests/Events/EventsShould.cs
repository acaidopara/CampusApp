using Rise.Domain.Events;
using Rise.Domain.Education;

namespace Rise.Domain.Tests.Events
{
    public class EventsShould
    {
        [Fact]
        public void Can_Create_Event_With_All_Required_Fields()
        {
            var slot = new EventTimeSlot(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(10, 0), new TimeOnly(12, 0));
            var address = new Address("Stationstraat", "10", "Gent", "9000");

            var evt = new Event
            {
                Title = "Open Campus Day",
                Date = slot,
                ImageUrl = "https://hogent.be/event.png",
                Subject = "Campus Event",
                Content = "Join us for a tour of the campus!",
                Address = address,
                Price = 0,
                RegisterLink = "https://hogent.be/register"
            };

            evt.ShouldNotBeNull();
            evt.Title.ShouldBe("Open Campus Day");
            evt.Date.ShouldBe(slot);
            evt.ImageUrl.ShouldBe("https://hogent.be/event.png");
            evt.Subject.ShouldBe("Campus Event");
            evt.Content.ShouldBe("Join us for a tour of the campus!");
            evt.Address.ShouldBe(address);
            evt.Price.ShouldBe(0);
            evt.RegisterLink.ShouldBe("https://hogent.be/register");
        }

        [Fact]
        public void Can_Create_Event_Without_Optional_Fields()
        {
            var slot = new EventTimeSlot(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(14, 0));
            var address = new Address("Stationstraat", string.Empty, "Gent", "9000"); // lege Addressline2

            var evt = new Event
            {
                Title = "Lecture Event",
                Date = slot,
                ImageUrl = "https://hogent.be/lecture.png",
                Subject = "Lecture",
                Content = "Guest lecture on AI",
                Address = address
            };

            evt.Price.ShouldBeNull();
            evt.RegisterLink.ShouldBeNull();
        }

        [Fact]
        public void Creating_Event_With_Null_Required_Fields_Should_Throw()
        {
            var slot = new EventTimeSlot(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(10, 0));
            var address = new Address("Stationstraat", "10", "Gent", "9000");

            Should.Throw<ArgumentNullException>(() =>
            {
                var evt = new Event
                {
                    Title = null!,
                    Date = slot,
                    ImageUrl = "https://hogent.be/event.png",
                    Subject = "Campus Event",
                    Content = "Content",
                    Address = address
                };
            });
        }
    }
}
