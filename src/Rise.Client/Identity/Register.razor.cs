using Microsoft.AspNetCore.Components;
using Rise.Shared.Identity.Accounts;

namespace Rise.Client.Identity;

public partial class Register
{
    [Inject] public required IAccountManager AccountManager { get; set; }

    private Result? _result;
    private AccountRequest.Register Model { get; set; } = new();

    public async Task RegisterUserAsync()
    {
        _result = await AccountManager.RegisterAsync(Model.Email!, Model.Password!, Model.ConfirmPassword!);

    }
}