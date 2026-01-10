using Rise.Domain.Education;

namespace Rise.Domain.Tests.Education
{
    public class AddressShould
    {
        [Fact]
        public void Can_Create_Address_With_Valid_Properties()
        {
            var address = new Address("Stationstraat", "10", "Gent", "9000");

            address.Addressline1.ShouldBe("Stationstraat");
            address.Addressline2.ShouldBe("10");
            address.City.ShouldBe("Gent");
            address.PostalCode.ShouldBe("9000");
        }

        [Theory]
        [InlineData(null, "City", "9000")]
        [InlineData("", "City", "9000")]
        [InlineData("   ", "City", "9000")]
        [InlineData("Street", null, "9000")]
        [InlineData("Street", "", "9000")]
        [InlineData("Street", "   ", "9000")]
        [InlineData("Street", "City", null)]
        [InlineData("Street", "City", "")]
        [InlineData("Street", "City", "   ")]
        public void Constructor_Should_Throw_On_NullOrWhitespace(string? line1, string? city, string? zip)
        {
            Should.Throw<ArgumentException>(() => new Address(line1!, "10", city!, zip!));
        }
        
        [Fact]
        public void Equality_Should_Work_Correctly()
        {
            var a1 = new Address("Main", "10", "Gent", "9000");
            var a2 = new Address("Main", "10", "Gent", "9000");
            var a3 = new Address("Other", "20", "Antwerp", "2000");

            a1.Equals(a2).ShouldBeTrue();
            a1.Equals(a3).ShouldBeFalse();
        }
    }
}
