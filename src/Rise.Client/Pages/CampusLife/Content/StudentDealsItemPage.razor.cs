using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;

namespace Rise.Client.Pages.CampusLife.Content;

public partial class StudentDealsItemPage : ComponentBase
{
    [Parameter] public int DealId { get; set; }
    private StudentDealDto.Detail? _deal;
    [Inject] public IStudentDealService StudentDealService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadDealAsync();
    }
    
    private async Task LoadDealAsync() 
    {
        var request = await StudentDealService.GetStudentDealByIdAsync(new GetByIdRequest.GetById
        {
            Id = DealId
        });
        _deal = request.IsSuccess? request.Value.StudentDeal:null;
    }
}