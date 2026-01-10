using Rise.Shared.Common;
using Rise.Shared.Departments;
using Rise.Client.Api;

namespace Rise.Client.Pages.Contact.Service;

public class DepartmentService(TransportProvider transport) : IDepartmentService
{
    public async Task<Result<DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request, CancellationToken ctx = default)
    {
        return (await transport.Current.PostAsync<DepartmentResponse.Create,DepartmentRequest.Create>("/api/departments", request, ctx))!;
    }

    public async Task<Result<DepartmentResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        return (await transport.Current.GetAsync<DepartmentResponse.Index>("/api/departments", cancellationToken: ctx))!;
    }
    public async Task<Result<DepartmentResponse.DetailExtended>> GetByIdAsync(GetByIdRequest.GetById request, CancellationToken ctx = default)
{
    return (await transport.Current.GetAsync<DepartmentResponse.DetailExtended>($"/api/departments/{request.Id}", cancellationToken: ctx))!;
}
    
}
