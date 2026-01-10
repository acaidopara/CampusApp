using MudBlazor;

namespace Rise.Client.MainComponents.Navigation;

public partial class NavbarHamburger
{
    private bool _isMenuOpen;
    private string CurrentIcon => _isMenuOpen ? Icons.Material.Filled.Close : Icons.Material.Filled.Menu;
    private void ToggleMenu() => _isMenuOpen = !_isMenuOpen;

    protected override void OnInitialized()
    {
        Navigation.LocationChanged += (_, _) =>
        {
            _isMenuOpen = false;
            InvokeAsync(StateHasChanged);
        };
    }
}