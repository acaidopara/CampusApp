using Microsoft.AspNetCore.Components;
using Rise.Client.Pages.CampusLife.Service;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.Common;

namespace Rise.Client.Pages.CampusLife.Content;

public partial class StudentClubPage : ComponentBase
{
    private List<StudentClubDto.Index> _clubItems = [];
    private string _searchTerm = "";
    private int _currentPage = 1;
    private int SelectedPage => _currentPage;
    private const int PageSize = 4;
    private int _pageCount = 1;
    private bool _isLoading = true;
    
    [Inject] public IStudentClubService StudentClubService { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadClubsAsync();
        _isLoading = false;
    }
    
    private async Task LoadClubsAsync()
    {
        var skip = (SelectedPage - 1) * PageSize;
        
        var request = new QueryRequest.SkipTake()
        {
            Skip = skip,
            Take = PageSize,
            SearchTerm = _searchTerm
        };

        var result = await StudentClubService.GetIndexAsync(request);
        if (result.IsSuccess)
        {
            _clubItems = result.Value.StudentClubs.ToList();
            
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
    
    private async Task OnSearchTermChanged(string searchTerm)
    {
        _searchTerm = searchTerm;
        _currentPage = 1;
        await LoadClubsAsync();
    }
    
    private async Task OnPageChanged(int newPage)
    {
        if (_currentPage == newPage) return;

        _currentPage = newPage;
        await LoadClubsAsync();
    }
}