
using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;

namespace Rise.Client.Pages.Deadlines.Content;

public partial class Index : ComponentBase
{
    private bool _isAuthenticated;

    private List<DeadlineDto.Index> _deadlines = new();

    private string _searchQuery = string.Empty;

    private string _selectedCourse = string.Empty;

    private string _sortOption = "Ascending";
    
    private bool _isLoading = true;

    private IEnumerable<string> Courses => _deadlines.Select(d => d.Course ?? "").Distinct();

    private IEnumerable<DeadlineDto.Index> FilteredDeadlines => _deadlines.Where(d =>
        (string.IsNullOrEmpty(_searchQuery) ||
         d.Title.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
         (d.Course?.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ?? false)) &&
        (string.IsNullOrEmpty(_selectedCourse) || (d.Course ?? "") == _selectedCourse));

    private IEnumerable<DeadlineDto.Index> UpcomingDeadlines =>
        SortDeadlines(FilteredDeadlines.Where(d => !d.IsCompleted));

    private IEnumerable<DeadlineDto.Index> SortDeadlines(IEnumerable<DeadlineDto.Index> dl) => _sortOption switch
    {
        "Ascending" => dl.OrderBy(d => d.DueDate),
        "Descending" => dl.OrderByDescending(d => d.DueDate),
        _ => dl.OrderBy(d => d.DueDate)
    };

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        StateHasChanged();
        
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        if (_isAuthenticated)
        {
            var request = new DeadlineRequest.GetForStudent
            {
                SearchTerm = string.Empty,
                Skip = 0,
                Take = 1000,
                OrderBy = string.Empty,
                OrderDescending = false
            };

            var result = await DeadlineService.GetIndexAsync(request);

            if (result.IsSuccess)
            {
                _deadlines = result.Value.Deadlines.ToList();
            }
        }

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private void UpdateUi()
    {
        StateHasChanged();
    }

    private void OnSearchTermChange(string value)
    {
        _searchQuery = value;
        StateHasChanged();
    }

    private void OnSortTermChange(string value)
    {
        _sortOption = value;
        StateHasChanged();
    }
}
