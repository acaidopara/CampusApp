using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;

namespace Rise.Client.Pages.Campus.Content;

public partial class CampusDetail
{
    [Parameter] public int CampusId { get; set; }
    private CampusDto.Detail? _campus;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var result = await InfrastructureService.GetCampusById(new GetByIdRequest.GetById
        {
            Id = CampusId
        });
        _campus = result.IsSuccess? result.Value.Campus:null;
        _isLoading = false;
    }
}