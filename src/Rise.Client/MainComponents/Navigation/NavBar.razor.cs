using Microsoft.AspNetCore.Components;

namespace Rise.Client.MainComponents.Navigation;

public partial class NavBar
{
    [Parameter] public bool ShowTopNav { get; set; }

    private bool IsProtectedDisabled(string href)
    {
        var isProtected = href is "deadlines" or "lessenrooster";
        return isProtected && !IsAuthenticated;
    }
}