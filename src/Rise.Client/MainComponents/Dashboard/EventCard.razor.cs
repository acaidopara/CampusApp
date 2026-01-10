using Microsoft.AspNetCore.Components;
using Rise.Shared.Events;

namespace Rise.Client.MainComponents.Dashboard;

public partial class EventCard
{
    [Parameter, EditorRequired] public EventDto.Detail Event { get; set; } = null!;
}