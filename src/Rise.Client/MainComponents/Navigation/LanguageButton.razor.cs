using System.Globalization;

namespace Rise.Client.MainComponents.Navigation;

public partial class LanguageButton
{
    private string _currentCulture = "nl";

    protected override async Task OnInitializedAsync()
    {
        var savedCulture = await LocalizationLocalStorageManager.GetBlazorCultureAsync();

        if (!string.IsNullOrWhiteSpace(savedCulture))
        {
            _currentCulture = savedCulture;
        }
    }

    private string GetLanguageClass(string lang)
        => _currentCulture == lang ? "bold" : "";

    private async Task OnCultureChangedAsync(string selectedCulture)
    {
        _currentCulture = selectedCulture;
        var cultureInfo = new CultureInfo(selectedCulture);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        await LocalizationLocalStorageManager.SetBlazorCultureAsync(selectedCulture);
        LocalizationService.InvokeLanguageChanged(cultureInfo);
        StateHasChanged();
    }
}