using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;
using Rise.Shared.Lessons;

namespace Rise.Client.Pages.Timetable.Content;

public partial class TimetableMobile
{
    [Parameter] public IEnumerable<LessonDto.Index> Lessons { get; set; } = [];
    [Parameter] public IEnumerable<DeadlineDto.Index> Deadlines { get; set; } = [];
    [Parameter] public DayOfWeek? SelectedDay { get; set; } = DayOfWeek.Monday;
    [Parameter] public DateTime WeekStart { get; set; }
    [Parameter] public int? OpenLessonId { get; set; }
    
    private LessonDto.Index? _selectedCourse;
    
    protected override void OnParametersSet()
    {
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
    
    private IEnumerable<LessonDto.Index> FilteredLessons =>
        Lessons.Where(l => l.Start.DayOfWeek == SelectedDay);
    
    private IEnumerable<DeadlineDto.Index> FilteredDeadlinesForDay
    {
        get
        {
            if (Deadlines is null) return [];
            var dayDate = WeekStart.AddDays(((int)SelectedDay! - (int)DayOfWeek.Monday));
            return Deadlines.Where(d => d.DueDate.Date == dayDate.Date);
        }
    }

    private void HandleDayChanged(string selectedDay)
    {
        SelectedDay = selectedDay switch
        {
            "Ma" => DayOfWeek.Monday,
            "Di" => DayOfWeek.Tuesday,
            "Wo" => DayOfWeek.Wednesday,
            "Do" => DayOfWeek.Thursday,
            "Vr" => DayOfWeek.Friday,
            _ => DayOfWeek.Monday
        };
    }

    private void ShowCourseDetails(int id)
    {
        _selectedCourse = Lessons.FirstOrDefault(l => l.Id == id);
    }

    private void CloseDetails()
    {
        _selectedCourse = null;
    }
}