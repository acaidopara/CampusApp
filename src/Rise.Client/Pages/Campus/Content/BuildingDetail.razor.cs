using Microsoft.AspNetCore.Components;
using Rise.Shared.Infrastructure;

namespace Rise.Client.Pages.Campus.Content;

public partial class BuildingDetail
{
    private BuildingDto.Detail? _building;
    private bool _isLoading = true;

    [Parameter] public int CampusId { get; set; }
    [Parameter] public int BuildingId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var request= await InfrastructureService.GetBuildingById(new BuildingRequest.GetById
        {
            CampusId = CampusId,
            BuildingId = BuildingId
        });
        _building = request.IsSuccess? request.Value.Building: null;
        _isLoading = false;
    }
}