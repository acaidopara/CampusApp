using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Server.Endpoints.Departments;

/// <summary>
/// List all departments.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="departmentService"></param>
public class Index(IDepartmentService departmentService) : Endpoint<QueryRequest.SkipTake, Result<DepartmentResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/departments");
        AllowAnonymous(); 
    }

    public override Task<Result<DepartmentResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return departmentService.GetIndexAsync(req, ct);
    }
}