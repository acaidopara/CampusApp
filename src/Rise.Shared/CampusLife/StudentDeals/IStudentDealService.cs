using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Shared.CampusLife.StudentDeals;

public interface IStudentDealService
{
    Task<Result<StudentDealResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnPromoCategory req, CancellationToken ct = default);
    Task<Result<StudentDealResponse.Detail>> GetStudentDealByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default);

}