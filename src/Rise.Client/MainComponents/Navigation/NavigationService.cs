using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Client.Identity;
using Rise.Shared.Notifications;
using System.Security.Claims;
using System.Threading;

namespace Rise.Client.MainComponents.Navigation;

public abstract class NavBase : ComponentBase, IDisposable
{
    [Inject] protected NavigationManager Navigation { get; set; } = null!;
    [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = null!;
    [Inject] protected NavigationService ProtectedNav { get; set; } = null!;
    [Inject] protected INotificationService NotificationService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] public required IStringLocalizer<Resources.Pages.Notifications.Notifications> Loca { get; set; }

    protected string ActiveHref = "/";
    protected bool IsAuthenticated;
    protected int _unreadCount;

    private Timer? _unreadRefreshTimer;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);

    private int _previousUnreadCount = -1;
    private int _lastNotifiedCount = -1;
    private DateTime _lastToastShownAtUtc = DateTime.MinValue;
    private readonly TimeSpan _toastSuppressWindow = TimeSpan.FromSeconds(10);

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        if (AuthProvider is CookieAuthenticationStateProvider cookieProvider)
            cookieProvider.AuthenticationChanged += OnAuthChanged;

        UpdateActiveHref();
        Navigation.LocationChanged += OnLocationChanged;

        if (ProtectedNav is not null)
            ProtectedNav.UnreadCountRefreshRequested += HandleUnreadCountRefresh;

        await LoadUnreadCount();

        _unreadRefreshTimer = new Timer(state =>
        {
            _ = InvokeAsync(async () =>
            {
                try
                {
                    await LoadUnreadCount();
                }
                catch
                {
                }
            });
        }, null, _refreshInterval, _refreshInterval);
    }

    private async Task HandleUnreadCountRefresh()
    {
        await LoadUnreadCount();
        StateHasChanged();
    }

    private void OnAuthChanged()
    {
        InvokeAsync(async () =>
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

            await LoadUnreadCount();
            StateHasChanged();
        });
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        UpdateActiveHref();
        StateHasChanged();
    }

    private void UpdateActiveHref()
    {
        var path = Navigation.ToBaseRelativePath(Navigation.Uri);
        ActiveHref = string.IsNullOrEmpty(path) ? "/" : path;
    }

    protected bool IsActive(string href)
    {
        return ActiveHref == href;
    }

    public void Dispose()
    {
        if (AuthProvider is CookieAuthenticationStateProvider cookieProvider)
            cookieProvider.AuthenticationChanged -= OnAuthChanged;
        
        Navigation.LocationChanged -= OnLocationChanged;

        if (ProtectedNav is not null)
            ProtectedNav.UnreadCountRefreshRequested -= HandleUnreadCountRefresh;

        _unreadRefreshTimer?.Dispose();
        _unreadRefreshTimer = null;
    }

    private async Task LoadUnreadCount()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user?.Identity?.IsAuthenticated is not true)
        {
            _previousUnreadCount = -1;
            _lastNotifiedCount = -1;
            _unreadCount = 0;
            StateHasChanged();
            return;
        }

        var userIdClaim = user.FindFirst("Id")?.Value
                          ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
        {
            StateHasChanged();
            return;
        }

        var request = new NotificationRequest.GetForUser
        {
            UserId = userId
        };

        var result = await NotificationService.GetUserUnreadCountAsync(request);
        var newCount = result.IsSuccess ? result.Value?.Count ?? 0 : 0;

        if (_previousUnreadCount >= 0 && newCount > _previousUnreadCount)
        {
            var now = DateTime.UtcNow;
            var shouldShowToast = _lastNotifiedCount != newCount
                                  || (now - _lastToastShownAtUtc) > _toastSuppressWindow;

            if (shouldShowToast)
            {
                var added = newCount - _previousUnreadCount;
                var baseMessage = Loca["NewNotification"].Value;

                Snackbar.Add(
                    baseMessage,
                    Severity.Info,
                    config =>
                    {
                        config.VisibleStateDuration = 15000;
                        config.ShowTransitionDuration = 250;
                        config.HideTransitionDuration = 50;
                        config.ShowCloseIcon = true;
                        config.Action = "Open";
                        config.OnClick = _ =>
                        {
                            Navigation.NavigateTo("/notificaties=,tab=Ongelezen");
                            return Task.CompletedTask;
                        };
                    });

                _lastNotifiedCount = newCount;
                _lastToastShownAtUtc = now;
            }
        }

        _unreadCount = newCount;
        _previousUnreadCount = newCount;
        StateHasChanged();
    }
}
public class NavigationService
{
    private readonly NavigationManager _navigation;
    private readonly IAccountManager _accountManager;
    private readonly ISnackbar _snackbar;
    private readonly IStringLocalizer<Resources.MainComponents.Navigation.Navigation> _loc; 

    public event Func<Task>? UnreadCountRefreshRequested;

    public NavigationService(
        NavigationManager navigation, 
        IAccountManager accountManager, 
        ISnackbar snackbar,
        IStringLocalizer<Resources.MainComponents.Navigation.Navigation> loc)  // Add this parameter
    {
        _navigation = navigation;
        _accountManager = accountManager;
        _snackbar = snackbar;
        _loc = loc;  
        _snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
    }

    public async Task RequestUnreadCountRefreshAsync()
    {
        if (UnreadCountRefreshRequested is not null)
            await UnreadCountRefreshRequested.Invoke();
    }

    public async Task NavigateTo(string href, string label)
    {
        var isProtected = href is "deadlines" or "lessenrooster";

        if (isProtected)
        {
            var isAuthenticated = await _accountManager.CheckAuthenticatedAsync();

            if (!isAuthenticated)
            {
                if (label == "Timetable")
                {
                    label = _loc["Timetable"];
                }
                string message = _loc["NotLoggedIn"];  
                _snackbar.Add(
                    $"{message} {label}.",
                    Severity.Normal,
                    config =>
                    {
                        config.VisibleStateDuration = 5000;
                        config.ShowTransitionDuration = 250;
                        config.HideTransitionDuration = 50;
                        config.ShowCloseIcon = false;
                        config.Action = "Login";
                        config.OnClick = _ =>
                        {
                            _navigation.NavigateTo("/login");
                            return Task.CompletedTask;
                        };
                    });
                return; 
            }
        }

        _navigation.NavigateTo(href);
    }
}