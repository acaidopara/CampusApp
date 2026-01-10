using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.Lessons;
using Rise.Shared.Notifications;
using Rise.Shared.Shortcuts;
using MudBlazor;

namespace Rise.Client.Pages;

public partial class Index
{
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = null!;
    [Inject] private ILessonService LessonService { get; set; } = null!;
    [Inject] private IShortcutService ShortcutService { get; set; } = null!;
    [Inject] private INotificationService NotificationService { get; set; } = null!;
    [Inject] private IAbsenceService AbsenceService { get; set; } = null!;

    private bool _isAuthenticated;
    private int? _userId;
    private LessonDto.Index? _lesson;
    private (string Title, string Message, Severity Severity, string? Link)? _alert;
    private List<string> _absence = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
        var user = authState.User;
        var userIdString = user.FindFirst("Id")?.Value;

        if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out var parsedId))
            _userId = parsedId;

        await FetchShortcutsAsync();
        await LoadCarouselAsync();
        await FetchAbsencesAsync();

        if (_isAuthenticated)
        {
            await FetchNextLessonAsync();
        }
        _isLoading = false;
    }
        
    private async Task FetchNextLessonAsync() {
        if (!_isAuthenticated || !_userId.HasValue)
            return;

        var result = await LessonService.GetNextLessonAsync();
        _lesson = result.IsSuccess ? result.Value.Lesson : null;
    }
    
    private async Task FetchAbsencesAsync()
    {
        if (!_isAuthenticated || !_userId.HasValue)
            return;
        
        var request = new AbsenceRequest.DayRequest
        {
            Day = DateTime.Now
        };
        var result = await AbsenceService.GetAbsencesForDay(request);

        _absence = result.IsSuccess ? result.Value.Absences.Select(a => a.TeacherName).ToList() : new List<string>();
    }

    private string GetQuicklinksUrl()
        => _isAuthenticated ? "/snelkoppelingen" : "/snelkoppelingen?guest=true";

    private async Task LoadCarouselAsync()
    {
        var result = await NewsService.GetCarouselAsync();
        if (result.IsSuccess)
            _carouselNews = result.Value.News.ToList();
    }

    private async Task FetchShortcutsAsync()
    {
        if (_userId.HasValue)
        {
            var request = new ShortcutRequest.GetForUser()
            {
                UserId = _userId.Value
            };

            var userShortcuts = await ShortcutService.GetUserShortcutsAsync(request);

            if (userShortcuts.IsSuccess)
            {
                _quicklinks = userShortcuts.Value.Shortcuts;
                return;
            }

            _quicklinks = new List<ShortcutDto.Index>();
            return;
        }

        var defaults = await ShortcutService.GetDefaultShortcuts(new QueryRequest.SkipTake());
        if (defaults.IsSuccess)
            _quicklinks = defaults.Value.Shortcuts;
    }

    private async Task CheckWarningNotificationsAsync()
    {
        if (!_userId.HasValue) return;

        var req = new NotificationRequest.GetForUser { UserId = _userId.Value };
        var res = await NotificationService.GetLastWarningNotificationAsync(req);

        if (res.IsSuccess)
        {
            _alert = (
                Title: res.Value.Notification.Title,
                Message: res.Value.Notification.Message,
                Severity: Severity.Error,
                Link: $"/notificaties/notificatie/{res.Value.Notification.Id}"
            );
        }
        else
        {
            _alert = null;
        }
    }
}