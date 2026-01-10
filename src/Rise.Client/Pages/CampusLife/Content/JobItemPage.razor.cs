using Microsoft.AspNetCore.Components;
using Rise.Client.Pages.CampusLife.Service;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Client.Pages.CampusLife.Content;

public partial class JobItemPage : ComponentBase
{
    [Parameter] public int JobId { get; set; }
    private JobDto.Detail? _job;
    [Inject] public IJobService JobService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadJobAsync();
    }
    
    private async Task LoadJobAsync() 
    {
        var request = await JobService.GetJobByIdAsync(new GetByIdRequest.GetById
        {
            Id = JobId
        });
        _job = request.IsSuccess? request.Value.Job:null;
    }
}