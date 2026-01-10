using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Common;

public partial class ErrorDisplay
{
    [Parameter] public required string Title { get; set; }
    [Parameter] public required string ErrorCode { get; set; }
    [Parameter] public required string Message { get; set; }
}