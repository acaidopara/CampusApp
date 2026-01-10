using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Dashboard;

public partial class ClassScheduleBlock
{
    [Parameter] public int? Id { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public DateTime Start { get; set; }
    [Parameter] public DateTime End { get; set; }
    [Parameter] public string? Room { get; set; }
    [Parameter] public string? LessonType { get; set; }
}