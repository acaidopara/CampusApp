using Microsoft.Extensions.DependencyInjection;
namespace Rise.Client.Api;

public abstract class AbstractServiceFactory<T>
{
    private readonly NetworkState _connectivityService;
    protected readonly IServiceProvider _serviceProvider;
    public event Action<T>? ServiceChanged;
    protected AbstractServiceFactory(IServiceProvider serviceProvider, NetworkState connectivityService)
    {
        _serviceProvider = serviceProvider;
        _connectivityService = connectivityService;
        _connectivityService.StatusChanged += OnNetworkStateChanged;
    }
    private void OnNetworkStateChanged()
    {
        var newService = Create();
        ServiceChanged?.Invoke(newService);
    }

    public T Create()
    {
        return _connectivityService.IsOnline
            ? CreateOnlineService()
            : CreateOfflineService();
    }


    protected abstract T CreateOnlineService();
    protected abstract T CreateOfflineService();
}