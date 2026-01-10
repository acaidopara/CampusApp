using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Departments;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Departments;
using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Services.Tests.Departments;

[Collection("IntegrationTests")]
public class DepartmentServiceShould : IAsyncLifetime 
{
    public required ApplicationDbContext Db;
    public required IDepartmentService DepartmentService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private DepartmentService CreateService(
        ApplicationDbContext db)
    {
        return new DepartmentService(db);
    }

    private Department CreateDepartment(string name, string description, DepartmentType type = DepartmentType.Department, Employee? manager = null)
    {
        return new Department
        {
            Name = name,
            Description = description,
            DepartmentType = type,
            Manager = manager
        };
    }

    private Employee CreateEmployee(string firstname, string lastname, string email, string title, Department department)
    {
        return new Employee
        {
            Firstname = firstname,
            Lastname = lastname,
            AccountId = "test-account",
            Department = department,
            Email = new EmailAddress(email),
            Birthdate = new DateTime(1980, 1, 1),
            Employeenumber = "EMP001",
            Title = title
        };
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDepartment()
    {
        DepartmentService = CreateService(Db);

        var request = new DepartmentRequest.Create
        {
            Name = "Test Department",
            Description = "Test Description"
        };

        var result = await DepartmentService.CreateAsync(request);

        Assert.True(result.IsCreated());
        Assert.NotEqual(0, result.Value.DepartmentId);

        var department = await Db.Departments.FirstOrDefaultAsync(d => d.Name == "Test Department");
        Assert.NotNull(department);
        Assert.Equal("Test Description", department.Description);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenNameExists()
    {
        var existing = CreateDepartment("Test Department", "Test Description");
        Db.Departments.Add(existing);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new DepartmentRequest.Create
        {
            Name = "Test Department",
            Description = "New Description"
        };

        var result = await DepartmentService.CreateAsync(request);

        Assert.True(result.IsConflict());
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnDepartments()
    {
        var dept1 = CreateDepartment("Dept1", "Desc1");
        var dept2 = CreateDepartment("Dept2", "Desc2");
        Db.Departments.AddRange(dept1, dept2);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Departments.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Dept1", result.Value.Departments.First().Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTermInName()
    {
        var dept1 = CreateDepartment("Dept1", "Desc1");
        var dept2 = CreateDepartment("Dept2", "Desc2");
        Db.Departments.AddRange(dept1, dept2);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            SearchTerm = "Dept1",
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Departments);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Dept1", result.Value.Departments.First().Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTermInDescription()
    {
        var dept1 = CreateDepartment("Dept1", "Desc1");
        var dept2 = CreateDepartment("Dept2", "Desc2");
        Db.Departments.AddRange(dept1, dept2);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            SearchTerm = "Desc2",
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Departments);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Dept2", result.Value.Departments.First().Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByNameAscendingByDefault()
    {
        var deptA = CreateDepartment("A Dept", "Desc");
        var deptC = CreateDepartment("C Dept", "Desc");
        var deptB = CreateDepartment("B Dept", "Desc");
        Db.Departments.AddRange(deptA, deptC, deptB);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var depts = result.Value.Departments.ToList();
        Assert.Equal(3, depts.Count);
        Assert.Equal("A Dept", depts[0].Name);
        Assert.Equal("B Dept", depts[1].Name);
        Assert.Equal("C Dept", depts[2].Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByNameDescending()
    {
        var deptA = CreateDepartment("A Dept", "Desc");
        var deptC = CreateDepartment("C Dept", "Desc");
        var deptB = CreateDepartment("B Dept", "Desc");
        Db.Departments.AddRange(deptA, deptC, deptB);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            OrderBy = "Name",
            OrderDescending = true,
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var depts = result.Value.Departments.ToList();
        Assert.Equal(3, depts.Count);
        Assert.Equal("C Dept", depts[0].Name);
        Assert.Equal("B Dept", depts[1].Name);
        Assert.Equal("A Dept", depts[2].Name);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldIncludeManager()
    {
        var dept = CreateDepartment("Dept", "Desc");
        Db.Departments.Add(dept);
        await Db.SaveChangesAsync();

        var user = CreateEmployee("John", "Doe", "john.doe@hogent.be", "Manager", dept);
        dept.Manager = user;
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var manager = result.Value.Departments.First().Manager;
        Assert.NotNull(manager);
        Assert.Equal("John", manager.FirstName);
        Assert.Equal("Doe", manager.LastName);
        Assert.Equal("john.doe@hogent.be", manager.Email);
        Assert.Equal("Manager", manager.Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldHandleNoManager()
    {
        var dept = CreateDepartment("Dept", "Desc");
        Db.Departments.Add(dept);
        await Db.SaveChangesAsync();

        DepartmentService = CreateService(Db);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await DepartmentService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Departments.First().Manager);
    }
}