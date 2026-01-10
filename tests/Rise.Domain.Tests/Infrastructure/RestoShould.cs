using Rise.Domain.Infrastructure;

namespace Rise.Domain.Tests.Infrastructure
{
    public class RestoShould
    {
        private Entities.Menu CreateMenu() => new Entities.Menu { StartDate = DateTime.Today };
        private Building CreateBuilding() => new Building { Name = "Building A", Campus = new Campus ()  };

        [Fact]
        public void Can_Create_Resto_With_Required_Fields()
        {
            var menu = CreateMenu();
            var building = CreateBuilding();

            var resto = new Resto
            {
                Menu = menu,
                Coordinates = "50.123,4.567",
                Name = "Resto A",
                Building = building,
            };

            resto.Menu.ShouldBe(menu);
            resto.Coordinates.ShouldBe("50.123,4.567");
            resto.Name.ShouldBe("Resto A");
            resto.Building.ShouldBe(building);
        }

        [Fact]
        public void Setting_Name_To_NullOrWhitespace_Should_Throw()
        {
            var resto = new Resto()
            {
                Menu = CreateMenu(),
                Coordinates = "50.123,4.567",
                Building = CreateBuilding(),
            };
            Should.Throw<ArgumentException>(() => resto.Name = null!);
            Should.Throw<ArgumentException>(() => resto.Name = "");
            Should.Throw<ArgumentException>(() => resto.Name = "   ");
        }
    }
}
