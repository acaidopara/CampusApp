using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.Common;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.CampusLife;


/// <summary>
/// List all studentclubs.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="studentClubService"></param>
public class IndexClubs(IStudentClubService studentClubService) : Endpoint<QueryRequest.SkipTake, Result<StudentClubResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/studentclubs");
        AllowAnonymous();
    }

    public override Task<Result<StudentClubResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return studentClubService.GetIndexAsync(req, ct);
    }
}