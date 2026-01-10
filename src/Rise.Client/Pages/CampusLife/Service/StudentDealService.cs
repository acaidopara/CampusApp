using System.Net.Http.Json;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Client.Api;

namespace Rise.Client.Pages.CampusLife.Service;

public class StudentDealService(TransportProvider _transport) : IStudentDealService
{
    public async Task<Result<StudentDealResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnPromoCategory request, CancellationToken ctx = default)
    {
        return (await _transport.Current.GetAsync<StudentDealResponse.Index>($"/api/studentdeals?{request.AsQuery()}", ctx))!;
    }
        
    public async Task<Result<StudentDealResponse.Detail>> GetStudentDealByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx)
    {
        return (await _transport.Current.GetAsync<StudentDealResponse.Detail>($"/api/studentdeals/{request.Id}", cancellationToken: ctx))!;
    }
}