using Microsoft.AspNetCore.Components;

namespace Rise.Client.Pages.Timetable.Components;

public partial class WeekSelector
{
    [Parameter] public EventCallback<DateTime> OnWeekChanged { get; set; }
    [Parameter] public DateTime? ForceWeekStart { get; set; }
    private int _week;
    private DateTime _currentDate = DateTime.Now;
    private readonly DateTime _startAcademicYear = new DateTime(2025, 9, 22);

    protected override void OnInitialized()
    {
        _week = CalculateAcademicWeek(_currentDate);
        NotifyParent();
    }

    protected override void OnParametersSet()
    {
        if (ForceWeekStart is not null)
        {
            _currentDate = ForceWeekStart.Value;
            _week = CalculateAcademicWeek(_currentDate);
        }
    }

    private int CalculateAcademicWeek(DateTime date)
    {
        var daysSinceStart = (date - _startAcademicYear).Days;
        if (daysSinceStart < 0)
            return 1;
        return (daysSinceStart / 7) + 1;
    }

    private async void NotifyParent()
    {
        var monday = _currentDate.AddDays(-(int)_currentDate.DayOfWeek + (int)DayOfWeek.Monday);
        if (OnWeekChanged.HasDelegate)
            await OnWeekChanged.InvokeAsync(monday);
    }

    private void NextWeek()
    {
        _currentDate = _currentDate.AddDays(7);
        _week = CalculateAcademicWeek(_currentDate);
        NotifyParent();
    }

    private void PreviousWeek()
    {
        _currentDate = _currentDate.AddDays(-7);
        _week = CalculateAcademicWeek(_currentDate);
        NotifyParent();
    }
}