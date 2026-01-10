using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rise.Shared.Student;

namespace Rise.Client.Pages.Settings.Components;

public partial class ColourSelector
{
    private readonly string[] _colourOptions =
    [
        "#FABC32", // Yellow classic
        "#16B0A5", // Turquoise
        "#F19DA0", // Pink
        "#EF8767", // Red/Orange
        "#BB90BD", // Purple
        "#4CA2D5" // Blue
    ];

    private string _selectedColour = "#FABC32";
    private bool _isSavingColour;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentColour();
    }

    private async Task LoadCurrentColour()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var colourClaim = user.FindFirst("PreferedColour");
                if (colourClaim != null && !string.IsNullOrEmpty(colourClaim.Value))
                {
                    _selectedColour = colourClaim.Value;
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading colour preference: {ex.Message}", Severity.Error);
        }
    }

    private async Task OnColourChange(string newColour)
    {
        if (_isSavingColour)
            return;

        _isSavingColour = true;
        var previousColour = _selectedColour;
        _selectedColour = newColour;

        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst("Id");
                if (userIdClaim == null)
                {
                    _selectedColour = previousColour;
                    return;
                }

                var userId = int.Parse(userIdClaim.Value);

                var colourRequest = new StudentRequest.Colour()
                {
                    UserId = userId,
                    ColourHex = _selectedColour
                };

                var response = await StudentService.UpdateColourPreference(colourRequest);

                if (response.IsSuccess)
                {
                    await Js.InvokeVoidAsync("setSecondaryColour", _selectedColour);
                    Snackbar.Add(Loc["ColourPreferenceUpdated"].Value, Severity.Success);

                    if (AuthenticationStateProvider is IHostEnvironmentAuthenticationStateProvider hostProvider)
                    {
                        hostProvider.SetAuthenticationState(
                            Task.FromResult(await AuthenticationStateProvider.GetAuthenticationStateAsync())
                        );
                    }
                }
                else if (!response.IsSuccess && response.Errors.Contains("Request not send"))
                {
                    Snackbar.Add(Loc["OfflineMessage"].Value, Severity.Error);
                    _selectedColour = previousColour;
                }
                else
                {
                    _selectedColour = previousColour;
                    Snackbar.Add(Loc["ColourPreferenceUpdateFailed"].Value, Severity.Error);
                }
            }
        }
        catch (Exception ex)
        {
            _selectedColour = previousColour;
            Snackbar.Add($"Error updating colour preference: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isSavingColour = false;
        }
    }
}