using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.CampusLife;


/// <summary>
/// List all deals.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="studentDealService"></param>
public class IndexDeals(IStudentDealService studentDealService) : Endpoint<TopicRequest.GetBasedOnPromoCategory, Result<StudentDealResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/studentdeals");
        AllowAnonymous();
    }

    public override Task<Result<StudentDealResponse.Index>> ExecuteAsync(TopicRequest.GetBasedOnPromoCategory req, CancellationToken ct)
    {
        return studentDealService.GetIndexAsync(req, ct);
    }
}