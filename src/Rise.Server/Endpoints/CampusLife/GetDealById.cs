using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.CampusLife;

/// <summary>
/// GET /api/studentdeals/{id}
/// Return a single studentdeal by id.
/// </summary>
public class GetDealById(IStudentDealService studentDealService) : Endpoint<GetByIdRequest.GetById, Result<StudentDealResponse.Detail>>
{
    public override void Configure()
    {
        Get("/api/studentdeals/{id}");
        AllowAnonymous();
    }

    public override Task<Result<StudentDealResponse.Detail>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return studentDealService.GetStudentDealByIdAsync(req, ct);
    }
}