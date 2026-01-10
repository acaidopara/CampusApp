using Ardalis.Result;
using Rise.Domain.Education;
using Rise.Domain.Entities;
using Rise.Domain.Infrastructure;
using Rise.Persistence;
using Rise.Services.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;

namespace Rise.Services.Tests.Infrastructure;
[Collection("IntegrationTests")]
public class InfrastructureServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required IInfrastructureService InfrastructureService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
        InfrastructureService = new InfrastructureService(Db);
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private static Campus CreateCampus(string name, string description = "Test Description", string imageUrl = "test.jpg", string mapsUrl = "maps.test", string tourUrl = "tour.test")
    {
        return new Campus
        {
            Name = name,
            Description = description,
            Address = new Address("Test Street 1","Test Street 2","Test City","1234"),
            Facilities = new CampusFacilities(false, false, false, false, false, false, false, false, false, false, false),
            ImageUrl = imageUrl,
            MapsUrl = mapsUrl,
            TourUrl = tourUrl
        };
    }

    private static Building CreateBuilding(string name, Campus campus, string description = "Test Description", string imageUrl = "test.jpg")
    {
        return new Building
        {
            Name = name,
            Campus = campus,
            Address = new Address("Test Street 1","Test Street 2","Test City","1234"),
            Facilities = new CampusFacilities(false, false, false, false, false, false, false, false, false, false, false),
            Description = description,
            ImageUrl = imageUrl
        };
    }

    private static Classroom CreateClassroom(string number, string name, string description, string category, string floor, Building building)
    {
        return new Classroom
        {
            Number = number,
            Name = name,
            Description = description,
            Category = category,
            Floor = floor,
            Building = building
        };
    }

    private static Domain.Infrastructure.Resto CreateResto(string name, string coordinates, Building building, Menu? menu = null)
    {
        return new Domain.Infrastructure.Resto
        {
            Name = name,
            Coordinates = coordinates,
            Building = building,
            Menu = menu ?? CreateMenu(false, "Dummy Menu")
        };
    }

    private static Menu CreateMenu(bool hasMenu, string descriptionMenu)
    {
        return new Menu
        {
            StartDate = new DateTime(DateOnly.MaxValue, TimeOnly.MaxValue),
            HasMenu = hasMenu,
            DescriptionMenu = descriptionMenu,
            Items = new Dictionary<string, List<MenuItem>>()
        };
    }

    private async Task<Campus> SetupCampus(string campusName)
    {
        var campus = CreateCampus(campusName);
        Db.Campuses.Add(campus);
        await Db.SaveChangesAsync();
        return campus;
    }

    private async Task<(Campus, Building)> SetupCampusWithBuilding(string campusName, string buildingName)
    {
        var campus = await SetupCampus(campusName);
        var building = CreateBuilding(buildingName, campus);
        Db.Buildings.Add(building);
        await Db.SaveChangesAsync();
        return (campus, building);
    }

    private async Task<(Campus, Building, Classroom)> SetupCampusWithBuildingAndClassroom(string campusName, string buildingName, string classroomNumber, string classroomName, string classroomDescription, string classroomCategory, string classroomFloor)
    {
        var (campus, building) = await SetupCampusWithBuilding(campusName, buildingName);
        var classroom = CreateClassroom(classroomNumber, classroomName, classroomDescription, classroomCategory, classroomFloor, building);
        Db.Classrooms.Add(classroom);
        await Db.SaveChangesAsync();
        return (campus, building, classroom);
    }

    private async Task<(Campus, Building, Domain.Infrastructure.Resto)> SetupCampusWithBuildingAndResto(string campusName, string buildingName, string restoName, string coordinates = "coord")
    {
        var (campus, building) = await SetupCampusWithBuilding(campusName, buildingName);
        var menu = CreateMenu(true, "Menu Desc");
        Db.Menus.Add(menu);
        await Db.SaveChangesAsync();
        var resto = CreateResto(restoName, coordinates, building, menu);
        Db.Restos.Add(resto);
        await Db.SaveChangesAsync();
        return (campus, building, resto);
    }

    [Fact]
    public async Task GetCampusesIndexAsync_ShouldReturnCampuses()
    {
        // Arrange
        await SetupCampus("Campus1");
        await SetupCampus("Campus2");

        var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetCampusesIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Campuses.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Campus1", result.Value.Campuses.First().Name);
    }

    [Fact]
    public async Task GetCampusesIndexAsync_ShouldFilterBySearchTerm()
    {
        // Arrange
        await SetupCampus("CampusA");
        await SetupCampus("CampusB");

        var request = new QueryRequest.SkipTake { SearchTerm = "CampusA", Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetCampusesIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Campuses);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("CampusA", result.Value.Campuses.First().Name);
    }

    [Fact]
    public async Task GetCampusesIndexAsync_ShouldOrderByNameDescending()
    {
        // Arrange
        await SetupCampus("CampusA");
        await SetupCampus("CampusC");
        await SetupCampus("CampusB");

        var request = new QueryRequest.SkipTake { OrderBy = "Name", OrderDescending = true, Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetCampusesIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        var campuses = result.Value.Campuses.ToList();
        Assert.Equal(3, campuses.Count);
        Assert.Equal("CampusC", campuses[0].Name);
        Assert.Equal("CampusB", campuses[1].Name);
        Assert.Equal("CampusA", campuses[2].Name);
    }

    [Fact]
    public async Task GetCampusesIndexAsync_ShouldSetHasRestoTrueIfRestosExist()
    {
        // Arrange
        await SetupCampusWithBuildingAndResto("CampusWithResto", "Building", "Resto");

        var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetCampusesIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Campuses);
        Assert.True(result.Value.Campuses.First().HasResto);
    }

    [Fact]
    public async Task GetBuildingsIndexAsync_ShouldReturnBuildings()
    {
        // Arrange
        var campus = await SetupCampus("Campus");
        var building1 = CreateBuilding("Building1", campus);
        var building2 = CreateBuilding("Building2", campus);
        Db.Buildings.AddRange(building1, building2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetBuildingsIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Buildings.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Building1", result.Value.Buildings.First().Name);
        Assert.Equal("Campus", result.Value.Buildings.First().Campus);
    }

    [Fact]
    public async Task GetBuildingsIndexAsync_ShouldFilterBySearchTerm()
    {
        // Arrange
        var campus = await SetupCampus("Campus");
        var building1 = CreateBuilding("BuildingA", campus);
        var building2 = CreateBuilding("BuildingB", campus);
        Db.Buildings.AddRange(building1, building2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake { SearchTerm = "BuildingA", Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetBuildingsIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Buildings);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("BuildingA", result.Value.Buildings.First().Name);
    }

    [Fact]
    public async Task GetClassroomsIndexAsync_ShouldReturnClassrooms()
    {
        // Arrange
        var (_, building, _) = await SetupCampusWithBuildingAndClassroom("Campus", "Building", "101", "Class1", "Desc1", "Lecture", "1");
        var classroom2 = CreateClassroom("102", "Class2", "Desc2", "Lab", "1", building);
        Db.Classrooms.Add(classroom2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetClassroomsIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Classrooms.Count());
        Assert.Equal(2, result.Value.TotalCount);
        var firstClass = result.Value.Classrooms.First();
        Assert.Equal("101", firstClass.Number);
        Assert.Equal("Building", firstClass.Building.Name);
        Assert.Equal("Campus", firstClass.Building.Campus);
    }

    [Fact]
    public async Task GetClassroomsIndexAsync_ShouldFilterBySearchTermOnDescriptionOrCategory()
    {
        // Arrange
        var (_, building, _) = await SetupCampusWithBuildingAndClassroom("Campus", "Building", "101", "Class1", "Desc1", "Lecture", "1");
        var classroom2 = CreateClassroom("102", "Class2", "Desc2", "Lab", "1", building);
        Db.Classrooms.Add(classroom2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake { SearchTerm = "Lecture", Skip = 0, Take = 10 };

        // Act
        var result = await InfrastructureService.GetClassroomsIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Classrooms);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Lecture", result.Value.Classrooms.First().Category);
    }
    
    [Fact]
    public async Task GetCampusById_ShouldReturnCampusDetail()
    {
        // Arrange
        var campus = await SetupCampus("TestCampus");

        var request = new GetByIdRequest.GetById { Id = campus.Id };

        // Act
        var result = await InfrastructureService.GetCampusById(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("TestCampus", result.Value.Campus.Name);
        Assert.Equal("Test Description", result.Value.Campus.Description);
        Assert.Equal("test.jpg", result.Value.Campus.ImageUrl);
        Assert.Equal("Test City", result.Value.Campus.Address.City);
    }

    [Fact]
    public async Task GetCampusById_ShouldReturnNotFoundIfInvalidId()
    {
        // Arrange
        var request = new GetByIdRequest.GetById { Id = 999 };

        // Act
        var result = await InfrastructureService.GetCampusById(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetBuildingById_ShouldReturnBuildingDetail()
    {
        // Arrange
        var (campus, building) = await SetupCampusWithBuilding("Campus", "TestBuilding");

        var request = new BuildingRequest.GetById { CampusId = campus.Id, BuildingId = building.Id };

        // Act
        var result = await InfrastructureService.GetBuildingById(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("TestBuilding", result.Value.Building.Name);
        Assert.Equal("Test Description", result.Value.Building.Description);
        Assert.Equal("test.jpg", result.Value.Building.ImageUrl);
        Assert.Equal("Campus", result.Value.Building.Campus);
    }

    [Fact]
    public async Task GetBuildingById_ShouldReturnNotFoundIfMismatch()
    {
        // Arrange
        var (_, building1) = await SetupCampusWithBuilding("Campus1", "Building1");
        var campus2 = await SetupCampus("Campus2");

        var request = new BuildingRequest.GetById { CampusId = campus2.Id, BuildingId = building1.Id };

        // Act
        var result = await InfrastructureService.GetBuildingById(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetClassroomById_ShouldReturnClassroom()
    {
        // Arrange
        var (campus, building, classroom) = await SetupCampusWithBuildingAndClassroom("Campus", "Building", "101", "TestClass", "Desc", "Category", "1");

        var request = new ClassroomRequest.GetById { CampusId = campus.Id, BuildingId = building.Id, ClassroomId = classroom.Id };

        // Act
        var result = await InfrastructureService.GetClassroomById(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Classrooms);
        Assert.Equal(1, result.Value.TotalCount);
        var dto = result.Value.Classrooms.First();
        Assert.Equal("101", dto.Number);
        Assert.Equal("TestClass", dto.Name);
        Assert.Equal("Desc", dto.Description);
        Assert.Equal("Category", dto.Category);
        Assert.Equal("1", dto.Floor);
        Assert.Equal("Building", dto.Building.Name);
        Assert.Equal("Campus", dto.Building.Campus);
    }

    [Fact]
    public async Task GetClassroomById_ShouldReturnNotFoundIfMismatch()
    {
        // Arrange
        var (_, _, classroom1) = await SetupCampusWithBuildingAndClassroom("Campus1", "Building1", "101", "Class1", "Desc", "Cat", "1");
        var (campus2, building2, _) = await SetupCampusWithBuildingAndClassroom("Campus2", "Building2", "102", "Class2", "Desc", "Cat", "1");

        var request = new ClassroomRequest.GetById { CampusId = campus2.Id, BuildingId = building2.Id, ClassroomId = classroom1.Id };

        // Act
        var result = await InfrastructureService.GetClassroomById(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetRestosFromCampus_ShouldReturnRestos()
    {
        // Arrange
        var (campus, building1, resto1) = await SetupCampusWithBuildingAndResto("Campus", "Building1", "Resto1");
        var menu2 = CreateMenu(true, "Menu Desc2");
        Db.Menus.Add(menu2);
        await Db.SaveChangesAsync();
        var resto2 = CreateResto("Resto2", "coord2", building1, menu2);
        Db.Restos.Add(resto2);
        await Db.SaveChangesAsync();

        var request = new GetByIdRequest.GetById { Id = campus.Id };

        // Act
        var result = await InfrastructureService.GetRestosFromCampus(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Restos.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Resto1", result.Value.Restos.First().Name);
        Assert.Equal("Campus", result.Value.Restos.First().CampusName);
        Assert.True(result.Value.Restos.First().Menu.HasMenu);
    }

    [Fact]
    public async Task GetRestosFromCampus_ShouldReturnNotFoundIfNoCampus()
    {
        // Arrange
        var request = new GetByIdRequest.GetById { Id = 999 };

        // Act
        var result = await InfrastructureService.GetRestosFromCampus(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains("Campus not found", result.Errors.First());
    }

    [Fact]
    public async Task GetRestosFromCampus_ShouldReturnNotFoundIfNoRestos()
    {
        // Arrange
        var campus = await SetupCampus("CampusWithoutResto");

        var request = new GetByIdRequest.GetById { Id = campus.Id };

        // Act
        var result = await InfrastructureService.GetRestosFromCampus(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains("Campus contains no restos", result.Errors.First());
    }
}