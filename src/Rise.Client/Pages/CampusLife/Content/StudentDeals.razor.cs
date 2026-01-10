using Microsoft.AspNetCore.Components;
using Rise.Client.MainComponents.Filtering;
using Rise.Shared.CampusLife;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Events;

namespace Rise.Client.Pages.CampusLife.Content;

public partial class StudentDeals : ComponentBase
{
    private List<StudentDealDto.Index> _deals = [];
    private List<FilterChipSet.FilterItem>? _filters;
    private string _searchTerm = "";
    private string? _selectedFilter = CategoriesPromo.All.Name;
    private int _currentPage = 1;
    private int SelectedPage => _currentPage;
    private const int PageSize = 4;
    private int _pageCount = 1;
    private bool _isLoading = true;
    [Inject] public IStudentDealService StudentDealService { get; set; } = default!;
    
    protected override void OnInitialized()
    {
        _filters = CategoriesPromo.AllCategories
            .Select(t => new FilterChipSet.FilterItem(t.Name, t.Icon))
            .ToList();
    }
    
    protected override async Task OnInitializedAsync()
    {
        await LoadDealsAsync();
        _isLoading = false;
    }

    private async Task LoadDealsAsync()
    {
        var skip = (SelectedPage - 1) * PageSize;
        
        var request = new TopicRequest.GetBasedOnPromoCategory()
        {
            Skip = skip,
            Take = PageSize,
            SearchTerm = _searchTerm,
            PromoCategory = _selectedFilter
        };

        var result = await StudentDealService.GetIndexAsync(request);
        if (result.IsSuccess)
        {
            _deals = result.Value.StudentDeals.ToList();
            
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
        _selectedFilter = newFilter ?? CategoriesPromo.All.Name;
        _currentPage = 1;
        await LoadDealsAsync();
    }
    
    // SEARCH FILTERING
    private async Task OnSearchTermChanged(string searchTerm)
    {
        _searchTerm = searchTerm;
        _currentPage = 1;
        await LoadDealsAsync();
    }
    
    private async Task OnPageChanged(int newPage)
    {
        if (_currentPage == newPage) return;
        _currentPage = newPage;
        await LoadDealsAsync();
    }
}