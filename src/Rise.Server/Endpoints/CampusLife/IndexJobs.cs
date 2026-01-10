using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.CampusLife;


/// <summary>
/// List all jobs.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="jobService"></param>
public class IndexJobs(IJobService jobService) : Endpoint<TopicRequest.GetBasedOnJobCategory, Result<JobResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/job");
        AllowAnonymous();
    }

    public override Task<Result<JobResponse.Index>> ExecuteAsync(TopicRequest.GetBasedOnJobCategory request, CancellationToken ct)
    {
        return jobService.GetIndexAsync(request, ct);
    }
}