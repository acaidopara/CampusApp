using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rise.Shared.Student;

namespace Rise.Client.Pages.Settings.Components;

public partial class CampusPreferenceSelector
{
    private string _selectedCampus = "";
    private bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentCampus();
    }

    private async Task LoadCurrentCampus()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var campusClaim = user.FindFirst("PreferedCampus");
                if (campusClaim != null && !string.IsNullOrEmpty(campusClaim.Value))
                {
                    _selectedCampus = campusClaim.Value;
                }
                else
                {
                    _selectedCampus = "Schoonmeersen";
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading campus preference: {ex.Message}", Severity.Error);
        }
    }

    private async Task OnCampusChanged(ChangeEventArgs e)
    {
        _isLoading = true;
        var oldCampus = _selectedCampus;
        _selectedCampus = e.Value?.ToString() ?? "Schoonmeersen";
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst("Id");
                if (userIdClaim == null)
                    return;

                var userId = int.Parse(userIdClaim.Value);

                var campusRequest = new StudentRequest.Campus
                {
                    UserId = userId,
                    CampusName = _selectedCampus
                };

                var response = await StudentService.UpdateCampusPreference(campusRequest);

                if (response.IsSuccess)
                {
                    Snackbar.Add(Loc["CampusPreferenceUpdated"].Value, Severity.Success);

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
                    _selectedCampus = oldCampus;
                }
                else
                {
                    Snackbar.Add(Loc["CampusPreferenceUpdateFailed"].Value, Severity.Error);
                    _selectedCampus = oldCampus;
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating campus preference: {ex.Message}", Severity.Error);
            _selectedCampus = oldCampus;
        }
        finally
        {
            _isLoading = false;
        }
    }
}