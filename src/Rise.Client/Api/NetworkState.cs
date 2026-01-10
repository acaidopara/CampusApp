using Microsoft.JSInterop;
using Rise.Shared.Api;

namespace Rise.Client.Api;
public class NetworkState : INetworkState, IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<NetworkState>? _dotNetRef;

    public event Action? StatusChanged;
    public bool IsOnline { get; private set; } = true;

    public NetworkState(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        IsOnline = await _js.InvokeAsync<bool>("blazorNetwork.check");
        await _js.InvokeVoidAsync("blazorNetwork.register", _dotNetRef);
    }

    [JSInvokable]
    public void SetOfflineStatus(bool isOffline)
    {
        IsOnline = !isOffline;
        StatusChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        await _js.InvokeVoidAsync("blazorNetwork.unregister");
        _dotNetRef?.Dispose();
    }
}
