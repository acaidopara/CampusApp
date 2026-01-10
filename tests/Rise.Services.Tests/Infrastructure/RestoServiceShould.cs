using Rise.Domain.Education;
using Rise.Domain.Infrastructure;
using Rise.Domain.Entities;
using Rise.Persistence;
using Rise.Services.Resto;
using Rise.Shared.Resto;

namespace Rise.Services.Tests.Infrastructure;
[Collection("IntegrationTests")]
public class RestoServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required IRestoService RestoService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private RestoService CreateService(ApplicationDbContext db)
    {
        return new RestoService(db);
    }
    private static Campus CreateCampus(string name)
    {
        return new Campus
        {
            Name = name,
            Address = new Address("Test Street 1", "Test Street 2", "Test City", "1234"),
            Facilities = new CampusFacilities(false, false, false, false, false, false, false, false, false, false, false)
        };
    }

    private static Building CreateBuilding(string name, Campus campus)
    {
        return new Building
        {
            Name = name,
            Campus = campus,
            Address = new Address("Test Street 1", "Test Street 2", "Test City", "1234"),
            Facilities = new CampusFacilities(false, false, false, false, false, false, false, false, false, false, false)
        };
    }

    private Domain.Infrastructure.Resto CreateResto(string name, string coordinates, Building building, Menu? menu = null)
    {
        return new Domain.Infrastructure.Resto
        {
            Name = name,
            Coordinates = coordinates,
            Building = building,
            Menu = menu ?? CreateMenu(false, "Dummy Menu")
        };
    }

    private Menu CreateMenu(bool hasMenu, string descriptionMenu)
    {
        return new Menu
        {
            HasMenu = hasMenu,
            DescriptionMenu = descriptionMenu,
            Items = new Dictionary<string, List<MenuItem>>(),
            StartDate = default
        };
    }

    private void AddMenuItem(Menu menu, string category, MenuItem item)
    {
        if (!menu.Items.ContainsKey(category))
        {
            menu.Items[category] = new List<MenuItem>();
        }
        menu.Items[category].Add(item);
    }

    private MenuItem CreateMenuItem(string name, string description, bool isVeganAndHalal, bool isVeggieAndHalal, FoodCategory category)
    {
        return new MenuItem
        {
            Name = name,
            Description = description,
            IsVeganAndHalal = isVeganAndHalal,
            IsVeggieAndHalal = isVeggieAndHalal,
            Category = category
        };
    }

    private async Task<(Campus, Building, Building)> SetupCampusWithTwoBuildings(string campusName, string building1Name, string building2Name)
    {
        var campus = CreateCampus(campusName);
        Db.Campuses.Add(campus);
        await Db.SaveChangesAsync();

        var building1 = CreateBuilding(building1Name, campus);
        var building2 = CreateBuilding(building2Name, campus);
        Db.Buildings.AddRange(building1, building2);
        await Db.SaveChangesAsync();

        return (campus, building1, building2);
    }

    private async Task<(Menu, Menu)> SetupTwoMenus(bool hasMenu1, string desc1, bool hasMenu2, string desc2)
    {
        var menu1 = CreateMenu(hasMenu1, desc1);
        var menu2 = CreateMenu(hasMenu2, desc2);
        Db.Menus.AddRange(menu1, menu2);
        await Db.SaveChangesAsync();
        return (menu1, menu2);
    }

    private async Task SetupTwoRestos(Building building1, Building building2, Menu menu1, Menu menu2, string resto1Name,
        string resto2Name, string coord = "coord")
    {
        var resto1 = CreateResto(resto1Name, coord, building1, menu1);
        var resto2 = CreateResto(resto2Name, coord, building2, menu2);
        Db.Restos.AddRange(resto1, resto2);
        await Db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnRestos()
    {
        var (_, building1, building2) = await SetupCampusWithTwoBuildings("Campus1", "Building1", "Building2");

        var (menu1, menu2) = await SetupTwoMenus(true, "Menu Desc1", true, "Menu Desc2");

        await SetupTwoRestos(building1, building2, menu1, menu2, "Resto1", "Resto2", "coord1");

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Restos.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Resto1", result.Value.Restos.First().Name);
        Assert.Equal("Campus1", result.Value.Restos.First().CampusName);
        Assert.Equal("coord1", result.Value.Restos.First().Coordinates);
        Assert.True(result.Value.Restos.First().Menu.HasMenu);
        Assert.Equal("Menu Desc1", result.Value.Restos.First().Menu.DescriptionMenu);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTermOnName()
    {
        var (_, building, _) = await SetupCampusWithTwoBuildings("Campus", "Building", "Unused");

        var (menu1, menu2) = await SetupTwoMenus(false, "test", false, "test2");

        await SetupTwoRestos(building, building, menu1, menu2, "Resto1", "Resto2");

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            SearchTerm = "Resto1",
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Restos);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Resto1", result.Value.Restos.First().Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTermOnBuildingName()
    {
        var (_, building1, building2) = await SetupCampusWithTwoBuildings("Campus", "Building1", "Building2");

        var (menu1, menu2) = await SetupTwoMenus(false, "test", false, "test2");

        await SetupTwoRestos(building1, building2, menu1, menu2, "Resto", "Resto");

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            SearchTerm = "Building1",
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Restos);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Resto", result.Value.Restos.First().Name);
        Assert.Equal("Campus", result.Value.Restos.First().CampusName);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTermOnCampusName()
    {
        var campus1 = CreateCampus("Campus1");
        var campus2 = CreateCampus("Campus2");
        Db.Campuses.AddRange(campus1, campus2);
        await Db.SaveChangesAsync();

        var building1 = CreateBuilding("Building", campus1);
        var building2 = CreateBuilding("Building", campus2);
        Db.Buildings.AddRange(building1, building2);
        await Db.SaveChangesAsync();

        var (menu1, menu2) = await SetupTwoMenus(false, "test", false, "test2");

        await SetupTwoRestos(building1, building2, menu1, menu2, "Resto", "Resto");

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            SearchTerm = "Campus1",
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Restos);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Campus1", result.Value.Restos.First().CampusName);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterVeganItems()
    {
        var (_, building, _) = await SetupCampusWithTwoBuildings("Campus", "Building", "Unused");

        var menu = CreateMenu(true, "Desc");
        AddMenuItem(menu, "Category1", CreateMenuItem("Item1", "Desc1", true, false, FoodCategory.Broodjes));
        AddMenuItem(menu, "Category1", CreateMenuItem("Item2", "Desc2", false, false, FoodCategory.Dessert));
        AddMenuItem(menu, "Category2", CreateMenuItem("Item3", "Desc3", true, true, FoodCategory.Groenten));
        Db.Menus.Add(menu);
        await Db.SaveChangesAsync();

        var resto = CreateResto("Resto", "coord", building, menu);
        Db.Restos.Add(resto);
        await Db.SaveChangesAsync();

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            IsVegan = true,
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Restos);
        var restoDto = result.Value.Restos.First();
        Assert.Equal(2, restoDto.Menu.Items.Count);
        Assert.Single(restoDto.Menu.Items["Category1"]);
        Assert.Equal("Item1", restoDto.Menu.Items["Category1"][0].Name);
        Assert.Single(restoDto.Menu.Items["Category2"]);
        Assert.Equal("Item3", restoDto.Menu.Items["Category2"][0].Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterVeggieItems()
    {
        var (_, building, _) = await SetupCampusWithTwoBuildings("Campus", "Building", "Unused");

        var menu = CreateMenu(true, "Desc");
        AddMenuItem(menu, "Category", CreateMenuItem("Item1", "Desc1", false, true, FoodCategory.Broodjes));
        AddMenuItem(menu, "Category", CreateMenuItem("Item2", "Desc2", false, false, FoodCategory.Dessert));
        Db.Menus.Add(menu);
        await Db.SaveChangesAsync();

        var resto = CreateResto("Resto", "coord", building, menu);
        Db.Restos.Add(resto);
        await Db.SaveChangesAsync();

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            IsVeggie = true,
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Restos);
        var restoDto = result.Value.Restos.First();
        Assert.Single(restoDto.Menu.Items);
        Assert.Single(restoDto.Menu.Items["Category"]);
        Assert.Equal("Item1", restoDto.Menu.Items["Category"][0].Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByIdAscendingByDefault()
    {
        var (_, building, _) = await SetupCampusWithTwoBuildings("Campus", "Building", "Unused");

        var (menuA, menuB) = await SetupTwoMenus(false, "test", false, "test2");

        await SetupTwoRestos(building, building, menuA, menuB, "RestoA", "RestoB");

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var restos = result.Value.Restos.ToList();
        Assert.Equal(2, restos.Count);
        Assert.True(restos[0].Id < restos[1].Id);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByNameDescending()
    {
        var (_, building, _) = await SetupCampusWithTwoBuildings("Campus", "Building", "Unused");

        var menuA = CreateMenu(false, "test");
        var menuB = CreateMenu(false, "test2");
        var menuC = CreateMenu(false, "test3");
        Db.Menus.AddRange(menuA, menuB, menuC);
        await Db.SaveChangesAsync();

        var restoA = CreateResto("A Resto", "coord", building, menuA);
        var restoC = CreateResto("C Resto", "coord", building, menuC);
        var restoB = CreateResto("B Resto", "coord", building, menuB);
        Db.Restos.AddRange(restoA, restoC, restoB);
        await Db.SaveChangesAsync();

        RestoService = CreateService(Db);

        var request = new SearchRequest.SkipTake
        {
            OrderBy = "Name",
            OrderDescending = true,
            Skip = 0,
            Take = 10
        };

        var result = await RestoService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var restos = result.Value.Restos.ToList();
        Assert.Equal(3, restos.Count);
        Assert.Equal("C Resto", restos[0].Name);
        Assert.Equal("B Resto", restos[1].Name);
        Assert.Equal("A Resto", restos[2].Name);
    }
}