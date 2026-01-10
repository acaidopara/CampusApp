using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Rise.Client.Pages.StudentCard.Content;

public partial class StudentCard : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
    
    private readonly DateTime _huidigeDatum = DateTime.Now;

    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await AuthProvider.GetAuthenticationStateAsync();
        _isLoading = false;
    }
}