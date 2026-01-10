using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Dashboard;

public partial class DateBlock
{
    [Parameter] public DateTime? Date { get; set; }
    [Parameter] public DateOnly? EventDate { get; set; }
    [Parameter] public string MonthFormat { get; set; } = "ddd";

    private DateTime DisplayDate =>
        EventDate?.ToDateTime(TimeOnly.MinValue) ?? Date ?? DateTime.MinValue;
}