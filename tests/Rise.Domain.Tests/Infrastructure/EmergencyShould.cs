using System;
using Xunit;
using Shouldly;
using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class EmergencyShould
    {
        private Campus CreateCampus() =>
            new Campus
            {
                Name = "Main Campus",
            };

        [Fact]
        public void Can_Create_Emergency_With_Required_Fields()
        {
            var campus = CreateCampus();
            var now = DateTime.UtcNow;
            var emergency = new Emergency
            {
                Title = "Fire Drill",
                Message = "Evacuate the building immediately",
                IsActive = true,
                DateAndTime = now,
                Campus = campus
            };

            emergency.Title.ShouldBe("Fire Drill");
            emergency.Message.ShouldBe("Evacuate the building immediately");
            emergency.IsActive.ShouldBeTrue();
            (emergency.DateAndTime - now).Duration().TotalSeconds.ShouldBeLessThan(1);
            emergency.Campus.ShouldBe(campus);
        }

        [Fact]
        public void Can_Update_Properties()
        {
            var campus = CreateCampus();
            var emergency = new Emergency
            {
                Title = "Old Title",
                Message = "Old message",
                IsActive = false,
                DateAndTime = DateTime.UtcNow.AddHours(-1),
                Campus = campus
            };

            var future = DateTime.UtcNow.AddHours(1);
            emergency.Title = "Updated Title";
            emergency.Message = "Updated message";
            emergency.IsActive = true;
            emergency.DateAndTime = future;

            emergency.Title.ShouldBe("Updated Title");
            emergency.Message.ShouldBe("Updated message");
            emergency.IsActive.ShouldBeTrue();
            (emergency.DateAndTime > DateTime.UtcNow).ShouldBeTrue();
        }

    }
}
