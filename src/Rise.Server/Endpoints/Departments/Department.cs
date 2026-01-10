using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Server.Endpoints.Departments;

public class Department(IDepartmentService departmentService) : Endpoint<GetByIdRequest.GetById, Result<DepartmentResponse.DetailExtended>>
{
    public override void Configure()
    {
        Get("/api/departments/{id}");
        AllowAnonymous();
    }

    public override Task<Result<DepartmentResponse.DetailExtended>> ExecuteAsync(GetByIdRequest.GetById req, CancellationToken ct)
    {
        return departmentService.GetByIdAsync(req, ct);
    }
}