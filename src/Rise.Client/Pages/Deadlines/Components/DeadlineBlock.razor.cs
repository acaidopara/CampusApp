// DeadlineBlock.razor.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Client.Pages.Deadlines.Service;
using Rise.Shared.Deadlines;

namespace Rise.Client.Pages.Deadlines.Components; // Namespace for client-side deadline components

/// <summary>
/// Partial class for the DeadlineBlock Razor component, handling logic for displaying and interacting with a single deadline.
/// This component manages expansion, completion toggling, and file uploads for a student's deadline.
/// </summary>
public partial class DeadlineBlock
{
    /// <summary>
    /// The deadline data transfer object to display.
    /// </summary>
    [Parameter] public DeadlineDto.Index Deadline { get; set; } = new();

    /// <summary>
    /// Event callback invoked when the completion status changes.
    /// </summary>
    [Parameter] public EventCallback OnCompletedChanged { get; set; }

    /// <summary>
    /// Flag indicating if a loading operation (e.g., API call) is in progress.
    /// </summary>
    private bool IsLoading { get; set; } = false;
}