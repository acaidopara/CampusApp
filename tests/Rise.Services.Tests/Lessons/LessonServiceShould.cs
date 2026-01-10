using System.Security.Claims;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Departments;
using Rise.Domain.Education;
using Rise.Domain.Infrastructure;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Lessons;
using Rise.Services.Tests.Fakers;
using Rise.Shared.Lessons;

namespace Rise.Services.Tests.Lessons;
[Collection("IntegrationTests")]
public class LessonServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required ILessonService LessonService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private static LessonService CreateService(ApplicationDbContext db, ClaimsPrincipal? user = null)
    {
        user ??= CreateUser("abc");
        var session = new FakeSessionContextProvider(user);
        return new LessonService(db, session);
    }
    
    private static ClaimsPrincipal CreateUser(string userId)
        => new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)], "testAuth"));
    
    
    
    
    // ============
    // SEED METHODS
    // ============
    
    private async Task SeedDepartmentsAsync()
    {
        var department = new Department
        {
            Name = "IT en Digitale Innovatie",
            Description = "Beschrijving",
            DepartmentType = DepartmentType.Department
        };

        var studyField = new StudyField
        {
            Name = "Toegepaste informatica",
            Departement = department
        };

        Db.Departments.Add(department);
        Db.StudyFields.Add(studyField);
        await Db.SaveChangesAsync();
    }
    
    private async Task<Building> SeedBuildingAsync()
    {
        var campus = new Campus
        {
            Name = "Campus Schoonmeersen",
            Description = "...",
            Address = new Address("Valentin Vaerwyckweg", "1", "9000", "Gent"),
            ImageUrl = "images/Schoonmeersen.jpg",
            TourUrl = "https://youreka-virtualtours.be/tours/hogent_schoonmeersen/",
            MapsUrl = "https://www.google.com/maps/...",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true)
        };

        var building = new Building
        {
            Name = "Gebouw B",
            Description = "...",
            Address = new Address(campus.Address.Addressline1, campus.Address.Addressline2, campus.Address.City, campus.Address.PostalCode),
            ImageUrl = "images/GebouwB.jpg",
            Campus = campus,
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, false, false, true, true)
        };

        Db.Buildings.Add(building);
        await Db.SaveChangesAsync();
        return building;
    }
    
    private async Task<(Course course, Teacher teacher, Classroom classroom, ClassGroup classGroup, Domain.Users.Student student)>
        SeedCourseWithTeacherClassroomAndStudentAsync(Building building)
    {
        var studyField = Db.StudyFields.Include(s => s.Departement).First();

        var course = new Course { Name = "RISE", StudyField = studyField };
        var classroom = new Classroom
        {
            Number = "010",
            Description = "GSCHB.0.010 BCON",
            Building = building
        };
        var teacher = new Teacher
        {
            Firstname = "John",
            Lastname = "Doe",
            AccountId = "teacher1",
            Department = studyField.Departement!,
            Email = new EmailAddress("John.Doe@hogent.be"),
            Birthdate = new DateTime(1980, 1, 1)
        };
        teacher.AddCourse(course);

        var classGroup = new ClassGroup { Name = "3A1" };
        var student = new Domain.Users.Student
        {
            Firstname = "Jane",
            Lastname = "Doe",
            AccountId = "student1",
            Department = studyField.Departement!,
            Email = new EmailAddress("jane@example.com"),
            Birthdate = new DateTime(2000, 1, 1),
            StudentNumber = "S12345"
        };
        
        Db.Courses.Add(course);
        Db.Classrooms.Add(classroom);
        Db.Teachers.Add(teacher);
        Db.Classgroups.Add(classGroup);
        Db.Students.Add(student);
        await Db.SaveChangesAsync();
        
        student.EnrollInCourse(course, classGroup);
        await Db.SaveChangesAsync();

        return (course, teacher, classroom, classGroup, student);
    }
    
    private async Task<Lesson> SeedLessonAsync(DateTime start, DateTime end, Course course, Teacher teacher, Classroom classroom, ClassGroup group)
    {
        var lesson = new Lesson(start, end, LessonType.Hoorcollege, course);
        lesson.AddTeacher(teacher);
        lesson.AddClassroom(classroom);
        lesson.AddClassGroup(group);

        Db.Lessons.Add(lesson);
        await Db.SaveChangesAsync();
        return lesson;
    }
    
    
    
    
    // ===================
    // GetIndexAsync tests
    // ===================

    [Fact]
    public async Task GetIndexAsync_ReturnsLessonsWithinDateRange()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (course, teacher, classroom, classGroup, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);

        var lessonInside = await SeedLessonAsync(
            new DateTime(2025, 11, 20, 10, 0, 0),
            new DateTime(2025, 11, 20, 12, 0, 0),
            course, teacher, classroom, classGroup);

        /* Lesson outside daterange */
        await SeedLessonAsync(
            new DateTime(2025, 12, 1, 10, 0, 0),
            new DateTime(2025, 12, 1, 12, 0, 0),
            course, teacher, classroom, classGroup);

        LessonService = CreateService(Db, CreateUser(student.AccountId));

        var request = new LessonRequest.Week
        {
            StartDate = new DateTime(2025, 11, 19),
            EndDate = new DateTime(2025, 11, 30)
        };

        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Lessons.Count().ShouldBe(1);
        result.Value.Lessons.First().Id.ShouldBe(lessonInside.Id);
    }

    [Fact]
    public async Task GetIndexAsync_NoLessons_ReturnsEmpty()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (_, _, _, _, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);

        LessonService = CreateService(Db, CreateUser(student.AccountId));

        var request = new LessonRequest.Week
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 31)
        };

        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Lessons.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }
    
    [Fact]
    public async Task GetIndexAsync_DefaultDates_ReturnsAllLessonsForStudent()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (course, teacher, classroom, classGroup, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);
        var lesson = await SeedLessonAsync(DateTime.Today, DateTime.Today.AddHours(2), course, teacher, classroom, classGroup);

        LessonService = CreateService(Db, CreateUser(student.AccountId));
        var request = new LessonRequest.Week();

        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Lessons.ShouldHaveSingleItem().Id.ShouldBe(lesson.Id);
    }

    [Fact]
    public async Task GetIndexAsync_EndDateBeforeStartDate_ReturnsInvalid()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (course, teacher, classroom, classGroup, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);
        await SeedLessonAsync(DateTime.Today.AddHours(10), DateTime.Today.AddHours(12), course, teacher, classroom, classGroup);

        LessonService = CreateService(Db, CreateUser(student.AccountId));
        var request = new LessonRequest.Week
        {
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today
        };

        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.Status.ShouldBe(ResultStatus.Invalid);
    }
    
    [Fact]
    public async Task GetIndexAsync_MultipleLessonsSameDay_ReturnsAllWithinRange()
    {
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (course, teacher, classroom, classGroup, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);

        var lesson1 = await SeedLessonAsync(DateTime.Today.AddHours(9), DateTime.Today.AddHours(10), course, teacher, classroom, classGroup);
        var lesson2 = await SeedLessonAsync(DateTime.Today.AddHours(11), DateTime.Today.AddHours(12), course, teacher, classroom, classGroup);

        LessonService = CreateService(Db, CreateUser(student.AccountId));

        var request = new LessonRequest.Week
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1)
        };

        var result = await LessonService.GetIndexAsync(request);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Lessons.Count().ShouldBe(2);
        result.Value.Lessons.Select(l => l.Id).ShouldContain(lesson1.Id);
        result.Value.Lessons.Select(l => l.Id).ShouldContain(lesson2.Id);
    }

    [Fact]
    public async Task GetIndexAsync_Unauthenticated_ReturnsUnauthorized()
    {
        // arrange
        var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        LessonService = CreateService(Db, unauthenticatedUser);
        var request = new LessonRequest.Week();
        
        // act
        var result = await LessonService.GetIndexAsync(request);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }
    
    [Fact]
    public async Task GetIndexAsync_NoStudentLinked_ReturnsNotFound()
    {
        // arrange
        var fakeUser = CreateUser("nonexistent");
        LessonService = CreateService(Db, fakeUser);
        var request = new LessonRequest.Week();
        
        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task GetIndexAsync_InvalidUserId_ReturnsConflict()
    {
        // arrange
        var fakeUser = CreateUser("");
        LessonService = CreateService(Db, fakeUser);
        var request = new LessonRequest.Week();
        
        // act
        var result = await LessonService.GetIndexAsync(request);

        // assert
        result.Status.ShouldBe(ResultStatus.Conflict);
    }

    // ===============================
    // GetNextLessonAsync tests
    // ===============================

    [Fact]
    public async Task GetNextLessonAsync_ReturnsNextLesson()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (course, teacher, classroom, classGroup, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);

        var now = DateTime.Now;
        var nextLesson = await SeedLessonAsync(now.AddHours(1), now.AddHours(2), course, teacher, classroom, classGroup);

        LessonService = CreateService(Db, CreateUser(student.AccountId));

        // act
        var result = await LessonService.GetNextLessonAsync();

        // assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Lesson.Id.ShouldBe(nextLesson.Id);
    }
    
    [Fact]
    public async Task GetNextLessonAsync_NoUpcomingLessons_ReturnsNotFound()
    {
        // arrange
        await SeedDepartmentsAsync();
        var building = await SeedBuildingAsync();
        var (_, _, _, _, student) = await SeedCourseWithTeacherClassroomAndStudentAsync(building);
        LessonService = CreateService(Db, CreateUser(student.AccountId));

        // act
        var result = await LessonService.GetNextLessonAsync();

        // assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task GetNextLessonAsync_Unauthenticated_ReturnsUnauthorized()
    {
        // arrange
        var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        LessonService = CreateService(Db, unauthenticatedUser);

        // act
        var result = await LessonService.GetNextLessonAsync();

        // assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }
    
    [Fact]
    public async Task GetNextLessonAsync_NoStudentLinked_ReturnsNotFound()
    {
        // arrange
        var fakeUser = CreateUser("nonexistent");
        LessonService = CreateService(Db, fakeUser);

        // act
        var result = await LessonService.GetNextLessonAsync();
        
        // assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task GetNextLessonAsync_InvalidUserId_ReturnsConflict()
    {
        // arrange
        var fakeUser = CreateUser("");
        LessonService = CreateService(Db, fakeUser);

        // act
        var result = await LessonService.GetNextLessonAsync();
        
        // assert
        result.Status.ShouldBe(ResultStatus.Conflict);
    }
}