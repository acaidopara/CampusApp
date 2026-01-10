using Rise.Shared.Departments;
using Rise.Shared.Identity;
using Rise.Domain.Departments;

namespace Rise.Server.Endpoints.Departments;

/// <summary>
/// Creation of a <see cref="Department"/>
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="departmentService"></param>
public class Create(IDepartmentService departmentService) : Endpoint<DepartmentRequest.Create, Result<DepartmentResponse.Create>>
{
    public override void Configure()
    {
        Post("/api/departments");
        Roles(AppRoles.Administrator); 
    }

    public override Task<Result<DepartmentResponse.Create>> ExecuteAsync(DepartmentRequest.Create req, CancellationToken ctx)
    {
        return departmentService.CreateAsync(req, ctx);
    }
}