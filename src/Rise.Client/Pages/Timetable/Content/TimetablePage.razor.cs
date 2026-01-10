using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using Rise.Shared.Deadlines;
using Rise.Shared.Lessons;

namespace Rise.Client.Pages.Timetable.Content;

public partial class TimetablePage
{
    [Parameter] public int? Id { get; set; }
    
    private DateTime _geselecteerdeWeekStart = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
    private DayOfWeek _dagVandaag = DateTime.Today.DayOfWeek;
    private IEnumerable<LessonDto.Index>? _gefilterdeLessen;
    private IEnumerable<DeadlineDto.Index>? _gefilterdeDeadlines;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await FilterLessenOpWeek();
        
        if (Id.HasValue && _gefilterdeLessen is not null)
        {
            var les = _gefilterdeLessen.FirstOrDefault(l => l.Id == Id.Value);
            if (les != null)
            {
                _dagVandaag = les.Start.DayOfWeek;
                _geselecteerdeWeekStart = les.Start.StartOfWeek(DayOfWeek.Monday);
            }
        }

        _isLoading = false;
    }
    
    private async Task HandleWeekChanged(DateTime newWeekStart)
    {
        _geselecteerdeWeekStart = newWeekStart;
        await FilterLessenOpWeek();
    }

    private async Task FilterLessenOpWeek()
    {
        var weekStart = _geselecteerdeWeekStart;
        var weekEinde = weekStart.AddDays(7);

        var weekRequest = new LessonRequest.Week
        {
            StartDate = weekStart,
            EndDate = weekEinde
        };
        var result = await LessonService.GetIndexAsync(weekRequest);
        _gefilterdeLessen = result.IsSuccess?result.Value.Lessons:null;

        var deadlineRequest = new DeadlineRequest.GetForStudent
        {
            StartDate = weekStart.ToString("yyyy-MM-dd"),
            EndDate = weekEinde.ToString("yyyy-MM-dd"),
            OrderBy = "DueDate"
        };

        var deadlineResult = await DeadlineService.GetIndexAsync(deadlineRequest);
        _gefilterdeDeadlines = deadlineResult.IsSuccess ? deadlineResult.Value.Deadlines : [];

        StateHasChanged();
    }
    
    private async Task GaNaarVandaag()
    {
        _geselecteerdeWeekStart = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
        _dagVandaag = DateTime.Today.DayOfWeek;
        Id = null;
        
        _gefilterdeLessen = _gefilterdeLessen?.ToList();
        StateHasChanged();
        
        await FilterLessenOpWeek();
    }
}