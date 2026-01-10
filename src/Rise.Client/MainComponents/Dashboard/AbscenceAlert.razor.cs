using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Dashboard;

public partial class AbscenceAlert
{
    [Parameter] public List<string>? Abscences { get; set; }
}