using Rise.Domain.Entities;

namespace Rise.Domain.Tests.Menu
{
    public class MenuShould
    {
        [Fact]
        public void Can_Create_Menu_And_AddMenuToDay()
        {
            var menu = new Entities.Menu { StartDate = DateTime.Today };
            var mondayItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Name = "Tomatensoep",
                    Description = "Verse soep",
                    IsVeganAndHalal = true,
                    IsVeggieAndHalal = false,
                    Category = FoodCategory.Soep
                }
            };

            menu.AddMenuToDay("Ma", mondayItems);

            menu.Items["Ma"].Count.ShouldBe(1);
            menu.Items["Ma"][0].Name.ShouldBe("Tomatensoep");
        }

        [Fact]
        public void AddMenuToDay_With_Invalid_Day_Should_Throw()
        {
            var menu = new Entities.Menu { StartDate = DateTime.Today };
            var items = new List<MenuItem>();

            Should.Throw<ArgumentException>(() => menu.AddMenuToDay(null!, items));
            Should.Throw<ArgumentException>(() => menu.AddMenuToDay("", items));
            Should.Throw<ArgumentException>(() => menu.AddMenuToDay("   ", items));
        }

        [Fact]
        public void ItemsJson_Should_Serialize_And_Deserialize_Correctly()
        {
            var menu = new Entities.Menu { StartDate = DateTime.Today };
            var item = new MenuItem
            {
                Name = "Tomatensoep",
                Description = "Verse soep",
                IsVeganAndHalal = true,
                IsVeggieAndHalal = false,
                Category = FoodCategory.Soep
            };
            menu.AddMenuToDay("Ma", new List<MenuItem> { item });

            var json = menu.ItemsJson;
            var newMenu = new Entities.Menu { StartDate = DateTime.Today }; // required property instellen
            newMenu.ItemsJson = json;

            newMenu.Items["Ma"].Count.ShouldBe(1);
            newMenu.Items["Ma"][0].Name.ShouldBe("Tomatensoep");
        }

        [Fact]
        public void Can_Set_And_Get_StartDate()
        {
            var menu = new Entities.Menu { StartDate = DateTime.Today };
            var newDate = DateTime.Today.AddDays(3);
            menu.StartDate = newDate;

            menu.StartDate.ShouldBe(newDate);
        }

        [Fact]
        public void AddMenuToDay_Should_Correctly_Capitalize_Day()
        {
            var menu = new Entities.Menu { StartDate = DateTime.Today };
            var items = new List<MenuItem>
            {
                new MenuItem { Name = "Salade", Description = "Verse salade", IsVeganAndHalal = true, IsVeggieAndHalal = true, Category = FoodCategory.Groenten }
            };

            menu.AddMenuToDay("ma", items);

            menu.Items.ContainsKey("Ma").ShouldBeTrue();
            menu.Items["Ma"].Count.ShouldBe(1);
            menu.Items["Ma"][0].Name.ShouldBe("Salade");
        }
    }
}
