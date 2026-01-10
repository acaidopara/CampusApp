using Rise.Shared.Common;

namespace Rise.Shared.Departments;

/// <summary>
/// Provides methods for managing department-related operations.
/// </summary>
public interface IDepartmentService
{
    Task<Result<Departments.DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request, CancellationToken ctx = default);
    Task<Result<Departments.DepartmentResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    Task<Result<Departments.DepartmentResponse.DetailExtended>> GetByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default);
}