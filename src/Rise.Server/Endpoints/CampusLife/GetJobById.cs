using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.CampusLife;

/// <summary>
/// GET /api/job/{id}
/// Return a single job by id.
/// </summary>
public class GetJobById(IJobService jobService) : Endpoint<GetByIdRequest.GetById, Result<JobResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/job/{id}");
        AllowAnonymous();
    }

    public override Task<Result<JobResponse.Detail>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return jobService.GetJobByIdAsync(req, ct);
    }
}