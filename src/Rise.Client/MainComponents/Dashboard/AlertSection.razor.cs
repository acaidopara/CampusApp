using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Rise.Client.MainComponents.Dashboard;

public partial class AlertSection
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Parameter] public required string Title { get; set; }
    [Parameter] public required string Message { get; set; }
    [Parameter] public Severity Severity { get; set; } = MudBlazor.Severity.Normal;
    [Parameter] public string? Link { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = false;
    [Parameter] public bool isOflineMessage { get; set; } = false;
    [Parameter] public string? SessionKey { get; set; }

    private bool IsHidden { get; set; } = false;

    private string StorageKey => SessionKey ?? $"alert_hidden_{Title?.GetHashCode() ?? 0}";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var val = await JS.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(val) && bool.TryParse(val, out var parsed) && parsed)
            {
                IsHidden = true;
            }
        }
        catch
        {

        }
    }

    private async void CloseAlert()
    {
        IsHidden = true;
        StateHasChanged();

        try
        {
            await JS.InvokeVoidAsync("sessionStorage.setItem", StorageKey, "true");
        }
        catch
        {

        }
    }
}