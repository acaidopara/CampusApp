using Rise.Shared.Common;

namespace Rise.Shared.CampusLife.StudentClub;

public interface IStudentClubService
{
    Task<Result<StudentClubResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ct = default);
}