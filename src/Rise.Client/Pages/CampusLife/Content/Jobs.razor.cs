using Microsoft.AspNetCore.Components;
using Rise.Client.MainComponents.Filtering;
using Rise.Shared.CampusLife;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Client.Pages.CampusLife.Content;

public partial class Jobs : ComponentBase
{
    
    private List<JobDto.Index> _jobs = [];
    private List<FilterChipSet.FilterItem>? _filters;
    private string _searchTerm = "";
    private string? _selectedFilter = CategoriesJob.All.Name;
    private int _currentPage = 1;
    private int SelectedPage => _currentPage;
    private const int PageSize = 4;
    private int _pageCount = 1;
    private bool _isLoading = true;
    [Inject] public IJobService JobService { get; set; } = default!;
    
    protected override void OnInitialized()
    {
        _filters = CategoriesJob.AllJobs
            .Select(t => new FilterChipSet.FilterItem(t.Name, t.Icon))
            .ToList();
    }
    
    protected override async Task OnInitializedAsync()
    {
        await LoadJobsAsync();
        _isLoading = false;
    }

    private async Task LoadJobsAsync()
    {
        var skip = (SelectedPage - 1) * PageSize;
        
        var request = new TopicRequest.GetBasedOnJobCategory()
        {
            Skip = skip,
            Take = PageSize,
            SearchTerm = _searchTerm,
            JobCategory = _selectedFilter
        };

        var result = await JobService.GetIndexAsync(request);
        if (result.IsSuccess)
        {
            _jobs = result.Value.Jobs.ToList();
            
            if (result.Value.TotalCount > 0)
            {
                _pageCount = (int)Math.Ceiling((double)result.Value.TotalCount / PageSize);
            }
            else
            {
                _pageCount = 1;
            }
        }
    }
    
    // CHIP FILTERING
    private async Task OnFilterChanged(string? newFilter)
    {
        _selectedFilter = newFilter ?? CategoriesJob.All.Name;
        _currentPage = 1;
        await LoadJobsAsync();
    }
    
    // SEARCH FILTERING
    private async Task OnSearchTermChanged(string searchTerm)
    {
        _searchTerm = searchTerm;
        _currentPage = 1;
        await LoadJobsAsync();
    }
    
    private async Task OnPageChanged(int newPage)
    {
        if (_currentPage == newPage) return;
        _currentPage = newPage;
        await LoadJobsAsync();
    }
    
}