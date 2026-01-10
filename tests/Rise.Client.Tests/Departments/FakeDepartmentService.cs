using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Client.Departments;

public class FakeDepartmentService : IDepartmentService
{
    public Task<Result<DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DepartmentResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        // var departments = Enumerable.Range(1, 5)
        //     .Select(i => new DepartmentDto.Index() { Id = i, Name = $"Department {i}", Description = $"Description {i}" });
        //
        // var wrapper = new DepartmentResponse.Index
        // {
        //     Departments = departments,
        //     TotalCount = 5,
        // };
        //
        // return Task.FromResult(Result.Success(wrapper));

        throw new NotImplementedException();
    }

    public Task<Result<DepartmentResponse.DetailExtended>> GetByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }
}

