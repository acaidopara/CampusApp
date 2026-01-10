using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
    
namespace Rise.Client.Identity;

public partial class Logout
{
    [Inject] public required IAccountManager AccountManager { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await AccountManager.CheckAuthenticatedAsync())
        {
            await AccountManager.LogoutAsync();
            Snackbar.Add(Loc["LoggedOut"].Value, Severity.Success);
            Navigation.NavigateTo("/");       
        }
    }
}