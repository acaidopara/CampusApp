using System.Net.Http.Json;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Client.Api;

namespace Rise.Client.Pages.CampusLife.Service
{
    public class JobService(TransportProvider _transport) : IJobService
    {
        public async Task<Result<JobResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnJobCategory request, CancellationToken ctx = default)
        {
            return (await _transport.Current.GetAsync<JobResponse.Index>($"/api/job?{request.AsQuery()}", ctx))!;
        }
        
        public async Task<Result<JobResponse.Detail>> GetJobByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx)
        {
            return (await _transport.Current.GetAsync<JobResponse.Detail>($"/api/job/{request.Id}", cancellationToken: ctx))!;
        }
        
    }
}