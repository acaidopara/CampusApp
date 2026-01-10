using Microsoft.AspNetCore.Components;

namespace Rise.Client.Pages.Shortcuts.Components;

public partial class QuicklinkManageItem : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string? Icon { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Colour { get; set; }
    [Parameter] public int Index { get; set; }

    [Parameter] public EventCallback<int> OnMoveLeft { get; set; }
    [Parameter] public EventCallback<int> OnMoveRight { get; set; }
    [Parameter] public EventCallback OnRemove { get; set; }
    [Parameter] public EventCallback<string> OnColourChanged { get; set; }
    
    private bool _showColourSelector;
    private void ToggleColourSelector()
    {
        _showColourSelector = !_showColourSelector;
    }

    private string _currentColour = "var(--secondary-color)";
    private readonly string[] _colorOptions =
    [
        "#FABC32", // Yellow classic
        "#16B0A5", // Turqoise
        "#F19DA0", // Pink
        "#EF8767", // Red/Orange
        "#BB90BD", // Purple
        "#4CA2D5"  // Blue
    ];

    protected override void OnParametersSet()
    {
        _currentColour = Colour ?? _colorOptions[0];
    }

    private async Task MoveLeft(int index)
    {
        _showColourSelector = false;

        if (OnMoveLeft.HasDelegate)
            await OnMoveLeft.InvokeAsync(index);
    }

    private async Task MoveRight(int index)
    {
        _showColourSelector = false;

        if (OnMoveRight.HasDelegate)
            await OnMoveRight.InvokeAsync(index);
    }

    private async Task RemoveQuicklink()
    {
        _showColourSelector = false;

        if (OnRemove.HasDelegate)
            await OnRemove.InvokeAsync();
    }

    private async Task SetColour(string c)
    {
        if (string.IsNullOrEmpty(c))
            return;

        _currentColour = c;
        _showColourSelector = false;

        if (OnColourChanged.HasDelegate)
            await OnColourChanged.InvokeAsync(c);
    }
}