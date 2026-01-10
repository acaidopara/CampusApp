using Rise.Client.Api;
using Rise.Shared.Api;

namespace Rise.Client.Api;
public class TransportProvider : ITransportProvider
{
    public IAppService Current { get; private set; }

    public event Action? TransportChanged;

    public TransportProvider(ServiceFactory factory)
    {
        Current = factory.Create();

        factory.ServiceChanged += async newTransport =>
        {
            await Current.ShutDown();
            Current = newTransport;
            TransportChanged?.Invoke();
        };
    }
}
