using Microsoft.AspNetCore.Components;
using Rise.Shared.Identity.Accounts;

namespace Rise.Client.Identity;

public partial class Login
{
    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private AccountRequest.Login _model = new();
    private Result _result = new();
    [Inject] public required IAccountManager AccountManager { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    public async Task LoginUser()
    {
        _result = await AccountManager.LoginAsync(_model.Email!, _model.Password!);

        if (_result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                Navigation.NavigateTo(ReturnUrl);
            }
            else
            {
                Navigation.NavigateTo("/");
            }
        }
    }

    private async Task DevLogin()
    {
        _model.Email = "jane.doe@student.hogent.be";
        _model.Password = "A1b2C3!";
        await LoginUser();
    }
}