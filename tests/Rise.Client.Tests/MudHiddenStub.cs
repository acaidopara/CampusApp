using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

namespace Rise.Client;

public class MudHiddenStub : IComponent
{
    private RenderHandle _renderHandle;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Breakpoint Breakpoint { get; set; }
    public static Breakpoint? TestBreakpoint { get; set; }
    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (!TestBreakpoint.HasValue || TestBreakpoint.Value == Breakpoint)
        {
            _renderHandle.Render(ChildContent ?? (builder => { }));
        }
        else
        {
            _renderHandle.Render(builder => { });
        }

        return Task.CompletedTask;
    }
}
