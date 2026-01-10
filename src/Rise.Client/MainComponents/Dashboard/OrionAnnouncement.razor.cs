using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Dashboard;

public partial class OrionAnnouncement
{
    [Parameter] public (string Title, string Cursus, string Description, string LinkUrl) Announcement { get; set; }
}