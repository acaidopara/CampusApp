using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Api.Interceptors;
public class ApiExceptionHandler : DelegatingHandler
{
    private readonly NavigationManager _nav;

    public ApiExceptionHandler(NavigationManager nav)
    {
        _nav = nav;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound && !request.RequestUri!.ToString().Contains("api/identity"))
        {
            _nav.NavigateTo("/404");
        }
        else if ((int)response.StatusCode >= 500)
        {
            _nav.NavigateTo("/error?code=500");
        }

        return response;
    }
}
