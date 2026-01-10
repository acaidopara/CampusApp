using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;
using Rise.Shared.Lessons;

namespace Rise.Client.Pages.Timetable.Content;

public partial class TimetableDesktop
{
    [Parameter] public IEnumerable<LessonDto.Index> Lessons { get; set; } = [];
    [Parameter] public IEnumerable<DeadlineDto.Index> Deadlines { get; set; } = [];
    [Parameter] public DateTime WeekStart { get; set; }

    private List<DayGroup> _weekDays = [];
    private List<string> _hours = ["8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"];
    private LessonDto.Index? _selectedCourse;
    [Parameter] public DayOfWeek? SelectedDay { get; set; } = DayOfWeek.Monday;
    [Parameter] public int? OpenLessonId { get; set; }

    protected override void OnParametersSet()
    {
        if (Lessons is null)
        {
            _weekDays = [];
            return;
        }

        var firstDayOfWeek = WeekStart.Date;

        _weekDays = Enumerable.Range(0, 5)
            .Select(i =>
            {
                var dayDate = firstDayOfWeek.AddDays(i);
                var lessonsOnDay = Lessons
                    .Where(l => l.Start.Date == dayDate.Date)
                    .OrderBy(l => l.Start)
                    .ToList();
                
                var deadlinesOnDay = Deadlines
                    .Where(d => d.DueDate.Date == dayDate.Date)
                    .OrderBy(d => d.DueDate)
                    .ToList();

                return new DayGroup
                {
                    Name = dayDate.ToString("ddd", new System.Globalization.CultureInfo("nl-BE")),
                    Date = dayDate,
                    Lessons = lessonsOnDay,
                    Deadlines = deadlinesOnDay
                };
            })
            .ToList();
        
        if (OpenLessonId.HasValue
            && Lessons != null
            && Lessons.Any()
            && _selectedCourse == null)
        {
            var found = Lessons.FirstOrDefault(l => l.Id == OpenLessonId.Value);
            if (found != null)
            {
                _selectedCourse = found;
                SelectedDay = found.Start.DayOfWeek;
            }
        }
    }

    private void ShowCourseDetails(int id)
    {
        _selectedCourse = Lessons.FirstOrDefault(l => l.Id == id);
    }

    private void CloseDetails()
    {
        _selectedCourse = null;
    }
    
    private class DayGroup
    {
        public string Name { get; set; } = default!;
        public DateTime Date { get; set; }
        public List<LessonDto.Index> Lessons { get; set; } = [];
        public List<DeadlineDto.Index> Deadlines { get; set; } = [];
    }
}