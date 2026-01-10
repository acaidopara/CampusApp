using Microsoft.AspNetCore.Components;
using Rise.Shared.Common; 
using Rise.Shared.Infrastructure; 

namespace Rise.Client.Pages.Campus.Content; 

public partial class Index 
{ 
    [Parameter] public int? CampusId { get; set; } 
    [Parameter] public int? BuildingId { get; set; } 
    [Parameter] public int? ClassroomId { get; set; } 

    private CampusDto.Index? _campus; 
    private bool _isSelectingCampus; 
    private List<CampusDto.Index>? _campuses; 
    private IEnumerable<CampusDto.Index>? _campusList; 
    private ClassroomDto.Index? _selectedClassroom; 
    private BuildingDto.Detail? _selectedBuilding;
    private bool _isLoading;
    private bool IsSearchActive { get; set; } 
    private string _searchTerm = ""; 
    private InfrastructureResponse.Index? _searchResults;
    

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        try
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync(); 
            var user = authState.User; 
            var campusPreference = user.FindFirst("PreferedCampus")?.Value; 

            var response = await InfrastructureService.GetCampusesIndexAsync(new QueryRequest.SkipTake()); 
            _campusList = response.Value.Campuses; 

            CampusDto.Index? preferredCampus = null; 
            if (!string.IsNullOrWhiteSpace(campusPreference)) 
            { 
                preferredCampus = _campusList.FirstOrDefault(c => 
                { 
                    var campusName = c.Name.Replace("Campus", "", StringComparison.OrdinalIgnoreCase).Trim(); 
                    var preferenceName = campusPreference.Replace("Campus", "", StringComparison.OrdinalIgnoreCase).Trim(); 
                    return string.Equals(campusName, preferenceName, StringComparison.OrdinalIgnoreCase); 
                }); 
            } 

            int effectiveCampusId = CampusId ?? (preferredCampus?.Id ?? 1); 

            var result = await InfrastructureService.GetCampusById(new GetByIdRequest.GetById { Id = effectiveCampusId }); 
            if (result.IsSuccess)
            {
                _campus = result.Value.Campus; 
            }
            else
            {
                return;
            }

            if (ClassroomId is not null && BuildingId is not null) 
            { 
                var classroomResult = await InfrastructureService.GetClassroomById(new ClassroomRequest.GetById() { CampusId = effectiveCampusId, BuildingId = BuildingId.Value, ClassroomId = ClassroomId.Value }); 
                if (classroomResult.IsSuccess) 
                { 
                    _selectedClassroom = classroomResult.Value.Classrooms.First(); 
                    _selectedBuilding = null; 
                }
            } 
            else if (BuildingId is not null) 
            { 
                var buildingResult = await InfrastructureService.GetBuildingById(new BuildingRequest.GetById { CampusId = effectiveCampusId, BuildingId = BuildingId.Value }); 
                if (buildingResult.IsSuccess) 
                { 
                    _selectedBuilding = buildingResult.Value.Building; 
                    _selectedClassroom = null; 
                }
            } 

            if (IsSearchActive)
            {
                CloseSearch();
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error loading data: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    } 

    private async Task OnSearchValueChanged(string newValue) 
    { 
        _searchTerm = newValue; 
        if (string.IsNullOrWhiteSpace(newValue))
        {
            CloseSearch();
        }
        else
        {
            IsSearchActive = true; 
            await FetchResults(newValue); 
        }
    } 

    private async Task FetchResults(string term) 
    { 
        var request = new QueryRequest.SkipTake { SearchTerm = term, Skip = 0, Take = 50, OrderBy = null, OrderDescending = false }; 
        var result = await InfrastructureService.GetInfrastructureIndexAsync(request); 
        if (result.IsSuccess) 
        { 
            _searchResults = result.Value; 
        } 
        else 
        { 
            _searchResults = null; 
        } 
    } 

    private void CloseSearch() 
    { 
        _searchTerm = ""; 
        IsSearchActive = false; 
        _searchResults = null; 
    } 

    private async Task SelectCampus() 
    { 
        _isSelectingCampus = true; 
        var request = new QueryRequest.SkipTake { SearchTerm = "", Skip = 0, Take = 100, OrderBy = "Name", OrderDescending = false }; 
        var result = await InfrastructureService.GetCampusesIndexAsync(request); 
        if (result.IsSuccess) 
        { 
            _campuses = result.Value.Campuses.ToList(); 
        } 
        else 
        { 
            _campuses = null; 
        } 
    } 

    private void CloseCampusSelection() 
    { 
        _isSelectingCampus = false; 
        _campuses = null; 
    } 

    private async Task SelectSpecificCampus(CampusDto.Index campus) 
    { 
        var result = await InfrastructureService.GetCampusById(new GetByIdRequest.GetById { Id = campus.Id }); 
        if (result.IsSuccess)
        {
            _campus = result.Value.Campus; 
            _selectedClassroom = null;
            _selectedBuilding = null;
        }
        else
        {
            return;
        }
        CloseCampusSelection(); 
    } 

} 