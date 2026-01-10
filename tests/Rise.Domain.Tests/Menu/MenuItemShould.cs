

using Rise.Domain.Entities;

namespace Rise.Domain.Tests.Menu
{
    public class MenuItemShould
    {
        [Fact]
        public void Can_Create_MenuItem_With_Valid_Fields()
        {
            var item = new MenuItem
            {
                Name = "Tomatensoep",
                Description = "Verse tomatensoep met kruiden",
                IsVeganAndHalal = true,
                IsVeggieAndHalal = false,
                Category = FoodCategory.Soep
            };

            item.Name.ShouldBe("Tomatensoep");
            item.Description.ShouldBe("Verse tomatensoep met kruiden");
            item.IsVeganAndHalal.ShouldBeTrue();
            item.IsVeggieAndHalal.ShouldBeFalse();
            item.Category.ShouldBe(FoodCategory.Soep);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Setting_Invalid_Name_Should_Throw(string invalidName)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var _ = new MenuItem { Name = invalidName, Description = "Desc", IsVeganAndHalal = true, IsVeggieAndHalal = false, Category = FoodCategory.Groenten };
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Setting_Invalid_Description_Should_Throw(string invalidDesc)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var _ = new MenuItem { Name = "Name", Description = invalidDesc, IsVeganAndHalal = true, IsVeggieAndHalal = false, Category = FoodCategory.Groenten };
            });
        }
    }
}
