using System.Security.Claims;
using Ardalis.Result;
using Rise.Domain.Departments;
using Rise.Domain.Education;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Deadlines;
using Rise.Services.Tests.Fakers;
using Rise.Shared.Deadlines;

namespace Rise.Services.Tests.Deadlines;
[Collection("IntegrationTests")]
public class DeadlineServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required IDeadlineService DeadlineService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private static DeadlineService CreateService(ApplicationDbContext db, ClaimsPrincipal? user = null)
    {
        user ??= CreateUser("abc");
        var session = new FakeSessionContextProvider(user);
        return new DeadlineService(db, session);
    }

    private static ClaimsPrincipal CreateUser(string userId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)],"testAuth"));
    }

    private static Domain.Users.Student CreateStudent(string accountId)
    {
        return new Domain.Users.Student()
        {
            Firstname = "Test",
            Lastname = "Test",
            AccountId = accountId,
            StudentNumber = "S123",
            PreferedCampus = "Main",
            Birthdate = new DateTime(2000, 1, 1),
            Department = new Department
            {
                Name = "IT en Digitale Innovatie",
                Description = "Opleidingen in toegepaste informatica, digitaal ontwerp en IT-beheer",
                DepartmentType = DepartmentType.Department
            },
            Email = new EmailAddress("test@student.hogent.be")
        };
    }

    private static Course CreateCourse()
    {
        return new Course
        {
            Name = "Programming",
            StudyField = new StudyField
            {
                Name = "IT",
                Departement = new Department
                {
                    Name = "IT en Digitale Innovatie",
                    Description = "Opleidingen in toegepaste informatica",
                    DepartmentType = DepartmentType.Department
                }
            }
        };
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        DeadlineService = CreateService(Db, unauthenticatedUser);
        var request = new DeadlineRequest.GetForStudent();

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsUnauthorized());
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnNotFound_WhenStudentNotExists()
    {
        var user = CreateUser("123");
        DeadlineService = CreateService(Db, user);

        var request = new DeadlineRequest.GetForStudent();

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnDeadlines()
    {
        var student = CreateStudent("abc");
        Db.Students.Add(student);

        var course = CreateCourse();
        Db.Courses.Add(course);

        var deadline = new Deadline
        {
            Title = "Exam",
            Course = course,
            DueDate = DateTime.UtcNow
        };
        Db.Deadlines.Add(deadline);

        Db.StudentDeadlines.Add(new StudentDeadline
        {
            Student = student,
            Deadline = deadline,
            IsCompleted = false
        });

        await Db.SaveChangesAsync();

        var user = CreateUser("abc");
        DeadlineService = CreateService(Db, user);

        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 10
        };

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Deadlines);
        Assert.Equal("Exam", result.Value.Deadlines.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTerm()
    {
        var student = CreateStudent("abc");
        Db.Students.Add(student);

        var course = CreateCourse();
        Db.Courses.Add(course);

        var exam = new Deadline { Title = "Exam", Course = course, DueDate = DateTime.UtcNow };
        var hw = new Deadline { Title = "Homework", Course = course, DueDate = DateTime.UtcNow };
        Db.Deadlines.AddRange(exam, hw);

        Db.StudentDeadlines.AddRange(
            new StudentDeadline { Student = student, Deadline = exam },
            new StudentDeadline { Student = student, Deadline = hw }
        );

        await Db.SaveChangesAsync();

        var user = CreateUser("abc");
        DeadlineService = CreateService(Db, user);

        var request = new DeadlineRequest.GetForStudent
        {
            SearchTerm = "Exam",
            Skip = 0,
            Take = 10
        };

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Deadlines);
        Assert.Equal("Exam", result.Value.Deadlines.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByDueDateAscending()
    {
        var student = CreateStudent("abc");
        Db.Students.Add(student);

        var course = CreateCourse();
        Db.Courses.Add(course);

        var d1 = new Deadline
        {
            Title = "D1",
            Course = course,
            DueDate = new DateTime(2025, 1, 10)
        };
        var d2 = new Deadline
        {
            Title = "D2",
            Course = course,
            DueDate = new DateTime(2025, 1, 5)
        };
        var d3 = new Deadline
        {
            Title = "D3",
            Course = course,
            DueDate = new DateTime(2025, 1, 20)
        };

        Db.Deadlines.AddRange(d1, d2, d3);

        Db.StudentDeadlines.AddRange(
            new StudentDeadline { Student = student, Deadline = d1 },
            new StudentDeadline { Student = student, Deadline = d2 },
            new StudentDeadline { Student = student, Deadline = d3 }
        );

        await Db.SaveChangesAsync();

        var user = CreateUser("abc");
        DeadlineService = CreateService(Db, user);

        var request = new DeadlineRequest.GetForStudent
        {
            OrderBy = "DueDate",
            OrderDescending = false,
            Skip = 0,
            Take = 10
        };

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);

        var deadlines = result.Value.Deadlines.ToList();

        Assert.Equal(3, deadlines.Count);
        Assert.Equal("D2", deadlines[0].Title);
        Assert.Equal("D1", deadlines[1].Title);
        Assert.Equal("D3", deadlines[2].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByDueDateDescending()
    {
        var student = CreateStudent("abc");
        Db.Students.Add(student);

        var course = CreateCourse();
        Db.Courses.Add(course);

        var d1 = new Deadline
        {
            Title = "D1",
            Course = course,
            DueDate = new DateTime(2025, 1, 10)
        };
        var d2 = new Deadline
        {
            Title = "D2",
            Course = course,
            DueDate = new DateTime(2025, 1, 5)
        };
        var d3 = new Deadline
        {
            Title = "D3",
            Course = course,
            DueDate = new DateTime(2025, 1, 20)
        };

        Db.Deadlines.AddRange(d1, d2, d3);

        Db.StudentDeadlines.AddRange(
            new StudentDeadline { Student = student, Deadline = d1 },
            new StudentDeadline { Student = student, Deadline = d2 },
            new StudentDeadline { Student = student, Deadline = d3 }
        );

        await Db.SaveChangesAsync();

        var user = CreateUser("abc");
        DeadlineService = CreateService(Db, user);

        var request = new DeadlineRequest.GetForStudent
        {
            OrderBy = "DueDate",
            OrderDescending = true,
            Skip = 0,
            Take = 10
        };

        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);

        var deadlines = result.Value.Deadlines.ToList();

        Assert.Equal(3, deadlines.Count);
        Assert.Equal("D3", deadlines[0].Title);
        Assert.Equal("D1", deadlines[1].Title);
        Assert.Equal("D2", deadlines[2].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterByCourse()
    {
        var student = CreateStudent("abc");
        Db.Students.Add(student);

        var course = CreateCourse();
        var course2 = new Course
        {
            Name = "RISE",
            StudyField = new StudyField
            {
                Name = "IT",
                Departement = new Department
                {
                    Name = "IT en Digitale Innovatie",
                    Description = "Opleidingen in toegepaste informatica",
                    DepartmentType = DepartmentType.Department
                }
            }
        };
        Db.Courses.AddRange(course, course2);

        var d1 = new Deadline
        {
            Title = "D1",
            Course = course,
            DueDate = new DateTime(2025, 1, 10)
        };
        var d2 = new Deadline
        {
            Title = "D2",
            Course = course2,
            DueDate = new DateTime(2025, 1, 5)
        };
        var d3 = new Deadline
        {
            Title = "D3",
            Course = course2,
            DueDate = new DateTime(2025, 1, 20)
        };

        Db.Deadlines.AddRange(d1, d2, d3);

        Db.StudentDeadlines.AddRange(
            new StudentDeadline { Student = student, Deadline = d1 },
            new StudentDeadline { Student = student, Deadline = d2 },
            new StudentDeadline { Student = student, Deadline = d3 }
        );

        await Db.SaveChangesAsync();

        var user = CreateUser("abc");
        DeadlineService = CreateService(Db, user);
        
        var request = new DeadlineRequest.GetForStudent
        {
            SearchTerm = "RISE",
            Skip = 0,
            Take = 10
        };
        
        var result = await DeadlineService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Deadlines.Count());
        Assert.All(result.Value.Deadlines, d => Assert.Equal("RISE", d.Course));
    }
}