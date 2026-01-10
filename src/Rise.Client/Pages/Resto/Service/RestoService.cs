using System.Net.Http.Json;
using System.Security.AccessControl;
using Rise.Shared.Infrastructure;
using Rise.Shared.Resto;
using Rise.Client.Api;

namespace Rise.Client.Pages.Resto.Service;

public class RestoService(TransportProvider _transport) : IRestoService
{
        public async Task<Result<RestoResponse.Index>> GetIndexAsync(SearchRequest.SkipTake request, CancellationToken ctx = default)
        {
                return (await _transport.Current.GetAsync<RestoResponse.Index>($"/api/restos?{request.AsQuery()}", cancellationToken: ctx))!;
        }
}
