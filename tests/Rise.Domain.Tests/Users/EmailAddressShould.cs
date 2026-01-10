using Rise.Domain.Users;

namespace Rise.Domain.Tests.Users
{
    public class EmailAddressShould
    {
        [Fact]
        public void Can_Create_EmailAddress_And_Normalize()
        {
            var email = new EmailAddress("Test@Example.COM  ");

            email.Value.ShouldBe("test@example.com");
        }

        [Fact]
        public void Equality_Should_Work_Correctly()
        {
            var e1 = new EmailAddress("a@b.com");
            var e2 = new EmailAddress("A@B.com");

            e1.Equals(e2).ShouldBeTrue();
            e1.GetHashCode().ShouldBe(e2.GetHashCode());
        }
    }
}
