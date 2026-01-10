using System;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Api;
using Rise.Shared.Api;

namespace Rise.Client.Api;

public class ServiceFactory : AbstractServiceFactory<IAppService>
{
    public ServiceFactory(IServiceProvider serviceProvider, NetworkState connectivityService)
        : base(serviceProvider, connectivityService)
    {
        
    }

    protected override IAppService CreateOnlineService()
    {
        return _serviceProvider.GetRequiredService<ApiClient>();
    }

    protected override IAppService CreateOfflineService()
    {
        return _serviceProvider.GetRequiredService<OfflineService>();
    }
}