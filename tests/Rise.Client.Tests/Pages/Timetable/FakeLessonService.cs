using Ardalis.Result;
using Rise.Shared.Events;
using Rise.Shared.Infrastructure;
using Rise.Shared.Lessons;

namespace Rise.Client.Pages.Timetable;

public class FakeLessonService : ILessonService
{
    private readonly int _delayMs;
    private DateTime _now;
    
    public FakeLessonService(int delayMs = 0)
    {
        _delayMs = delayMs;
        
        var monday = DateTime.Today;
        while (monday.DayOfWeek != DayOfWeek.Monday)
            monday = monday.AddDays(-1);

        _now = new DateTime(monday.Year, monday.Month, monday.Day, 8, 15, 0);
    }
    
    private static ClassroomDto.Index ExampleClassroom(int id = 1, string number = "010", string name = "01.02.100.010")
    {
        return new ClassroomDto.Index
        {
            Id = id,
            Number = number,
            Name = name,
            Description = $"GSCHB.0.010 BCON {name}",
            Category = "auditorium",
            Floor = "0",
            Building = new BuildingDto.Index
            {
                Id = 1,
                Name = "Gebouw B",
                Address = new AddressDto.Index
                {
                    AddressLine1 = "Valentin Vaerwyckweg",
                    City = "Gent",
                    PostalCode = "9000"
                },
                CampusId = 1,
            }
        };
    }

    public async Task<Result<LessonResponse.Index>> GetIndexAsync(LessonRequest.Week request,
        CancellationToken ctx = default)
    {
        if (_delayMs > 0) 
            await Task.Delay(_delayMs);
        
        var lessons = new List<LessonDto.Index>
        {
            new LessonDto.Index
            {
                Id = 1,
                Name = "RISE",
                Start = _now.AddHours(1),
                End = _now.AddHours(3),
                ClassroomDtos = [ExampleClassroom()],
                TeacherNames = ["John Doe"],
                ClassgroupNames = ["3A1"],
                LessonType = "Werkcollege"
            },
            new LessonDto.Index
            {
                Id = 2,
                Name = "Modern Data Architectures",
                Start = _now.AddHours(4),
                End = _now.AddHours(7),
                ClassroomDtos = [ExampleClassroom(2, "020", "01.02.100.020")],
                TeacherNames = ["Jane Smith"],
                ClassgroupNames = ["3A2"],
                LessonType = "Hoorcollege"
            },
            new LessonDto.Index
            {
                Id = 3,
                Name = "ITPCO",
                Start = _now.AddDays(1).AddHours(1),
                End = _now.AddDays(1).AddHours(3),
                ClassroomDtos = [ExampleClassroom(3, "030", "01.02.100.030")],
                TeacherNames = ["Bob Peters"],
                ClassgroupNames = ["3A1", "3A2"],
                LessonType = "Hoorcollege"
            }
        };

        return Result.Success(new LessonResponse.Index
        {
            Lessons = lessons,
            TotalCount = lessons.Count
        });
    }
    
    public async Task<Result<LessonResponse.NextLesson>> GetNextLessonAsync(
        CancellationToken ctx = default)
    {
        if (_delayMs > 0)
            await Task.Delay(_delayMs, ctx);

        var nextLesson = new LessonDto.Index
        {
            Id = 2,
            Name = "RISE",
            Start = _now.AddHours(2),
            End = _now.AddHours(4),
            ClassroomDtos = [ExampleClassroom(4, "040", "01.02.100.040")],
            TeacherNames = ["John Doe"],
            ClassgroupNames = ["3A1"],
            LessonType = "Hoorcollege"
        };

        return Result.Success(new LessonResponse.NextLesson
        {
            Lesson = nextLesson
        });
    }
}