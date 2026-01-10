using Rise.Client.MainComponents.Filtering;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Client.Components.Events;

public partial class EventContent
{
    private List<EventDto.Detail> _eventItems = [];
    private List<EventDto.Index> _carouselEvents = [];
    private List<FilterChipSet.FilterItem>? _filters;
    private string _searchTerm = "";
    private string? _selectedFilter = Topics.All.Name;
    private int _currentPage = 1;
    private int SelectedPage => _currentPage;
    private const int PageSize = 4;
    private int _pageCount = 1;

    protected override void OnInitialized()
    {
        _filters = Topics.AllTopics
            .Select(t => new FilterChipSet.FilterItem(t.Name, t.Icon))
            .ToList();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCarouselAsync();
        await LoadEventsAsync();
    }

    private async Task LoadCarouselAsync()
    {
        var result = await EventService.GetCarouselAsync();
        if (result.IsSuccess)
        {
            _carouselEvents = result.Value.Events.ToList();
        }
    }

    private async Task LoadEventsAsync()
    {
        var skip = (SelectedPage - 1) * PageSize;

        var request = new TopicRequest.GetBasedOnTopic()
        {
            Skip = skip,
            Take = PageSize,
            SearchTerm = _searchTerm,
            Topic = _selectedFilter
        };

        var result = await EventService.GetIndexAsync(request);
        if (result.IsSuccess)
        {
            _eventItems = result.Value.Events.ToList();

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

    private async Task OnFilterChanged(string? newFilter)
    {
        _selectedFilter = newFilter ?? Topics.All.Name;
        _currentPage = 1;
        await LoadEventsAsync();
    }

    private async Task OnSearchTermChanged(string searchTerm)
    {
        _searchTerm = searchTerm;
        _currentPage = 1;
        await LoadEventsAsync();
    }

    private async Task OnPageChanged(int newPage)
    {
        if (_currentPage == newPage) return;

        _currentPage = newPage;
        await LoadEventsAsync();
    }
}