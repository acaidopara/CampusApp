namespace Rise.Shared.Api;

public interface ITransportProvider
{
    IAppService Current { get; }
    event Action? TransportChanged;
}