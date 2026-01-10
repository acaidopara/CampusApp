using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Shared.CampusLife.Jobs;

public interface IJobService
{
    Task<Result<JobResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnJobCategory request, CancellationToken ctx = default);
    Task<Result<JobResponse.Detail>> GetJobByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default);
}