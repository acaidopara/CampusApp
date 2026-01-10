using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Client.MainComponents.Filtering;
using Rise.Client.MainComponents.Navigation;
using Rise.Shared.Notifications;

namespace Rise.Client.Pages.Notification.Content
{
    public class NotificationsBase : ComponentBase
    {
        [Inject] public required INotificationService NotificationService { get; set; }
        [Inject] public required AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] public required NavigationManager NavManager { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required IStringLocalizer<Resources.Pages.Notifications.Notifications> Loc { get; set; }
        [Inject] public required NavigationService ProtectedNav { get; set; }
        protected bool _isLoading = true;
        protected IEnumerable<NotificationDto.Index> Notifications { get; private set; } = new List<NotificationDto.Index>();

        private string _searchTerm = string.Empty;
        private int? UserId { get; set; }
        public int UnreadCount = 0;
        protected string? SelectedFilter;
        protected List<FilterChipSet.FilterItem>? Filters;


        private int _currentPage = 1;
        protected int SelectedPage => _currentPage;
        protected const int PageSize = 10;
        protected int _pageCount = 1;
        

        protected override void OnInitialized()
        {
            Filters = [
            new (Loc["Alles"], null),
            new (Loc["Gelezen"], "fa-solid fa-envelope-circle-check"),
            new (Loc["Ongelezen"], "fa-solid fa-envelope")];

            SelectedFilter = Loc["Alles"];
        }

        protected override async Task OnInitializedAsync()
        {
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            if (query["guest"] == "true")
            {
                Snackbar.Add(Loc["LoginRequired"].Value, Severity.Warning);
            }

            var tab = query["tab"];
            if (!string.IsNullOrEmpty(tab))
            {
                SelectedFilter = tab;
            }

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userIdString = user.FindFirst("Id")?.Value;

            if (int.TryParse(userIdString, out var parsedId))
                UserId = parsedId;

            await FetchNotificationsAsync();
            _isLoading = false;
        }

        protected async Task OnFilterChanged(string? newFilter)
        {
            if (newFilter != null)
            {
                SelectedFilter = newFilter;
                _currentPage = 1; 
                await LoadNotificationsAsync();
            }
        }

        protected async Task OnSearchTermChanged(string searchTerm)
        {
            _searchTerm = searchTerm ;
            _currentPage = 1;
            await LoadNotificationsAsync();
        }

        private async Task FetchNotificationsAsync()
        {
            var skip = (SelectedPage - 1) * PageSize;
            if (UserId.HasValue && SelectedFilter != null)
            {
                var request = new NotificationRequest.GetForUser() { 
                    UserId = UserId.Value, 
                    TopicTerm = SelectedFilter,
                    Skip = skip,
                    Take = PageSize,
                };
                var result = await NotificationService.GetUserNotificationsAsync(request);
                Notifications = result.IsSuccess ? result.Value.Notifications : new List<NotificationDto.Index>();
                if (result.Value.TotalCount > 0)
                {
                    _pageCount = (int)Math.Ceiling((double)result.Value.TotalCount / PageSize);
                }
                else
                {
                    _pageCount = 1;
                }
            }
            await GetUserUnreadCountAsync();
        }

        private async Task LoadNotificationsAsync()
        {
            if (!UserId.HasValue) return;
            if (SelectedFilter == null) return;

            var skip = (SelectedPage - 1) * PageSize;
            var request = new NotificationRequest.GetForUser()
            {
                SearchTerm = _searchTerm,
                UserId = UserId.Value,
                TopicTerm = SelectedFilter,
                Skip = skip,
                Take = PageSize,
            };

            var result = await NotificationService.GetUserNotificationsAsync(request);
            if (result.IsSuccess) { 
                Notifications = result.Value.Notifications.ToList();

                if (result.Value.TotalCount > 0)
                {
                    _pageCount = (int)Math.Ceiling((double)result.Value.TotalCount / PageSize);
                }
                else
                {
                    _pageCount = 1;
                }
            }
            await GetUserUnreadCountAsync();
        }

        protected async Task HandleToggleRead(int id)
        {
            if (!UserId.HasValue) return;

            var request = new NotificationRequest.ChangeRead
            {
                UserId = UserId.Value,
                NotificationId = id
            };

            var result = await NotificationService.UpdateUserNotificationIsReadAsync(request);
            if (!result.IsSuccess)
            {
                Snackbar.Add(Loc["MarkAsReadError"].Value, Severity.Error);
                return;
            }

            var list = Notifications.ToList();
            var idx = list.FindIndex(n => n.Id == id);
            if (idx >= 0)
            {
                list[idx].IsRead = true;
                if (SelectedFilter == Loc["Ongelezen"])
                {
                    list.RemoveAt(idx);
                    UnreadCount = Math.Max(0, UnreadCount - 1);
                }
                Notifications = list;
                StateHasChanged();

                await ProtectedNav.RequestUnreadCountRefreshAsync();
            }
        }

        protected async Task OnArticleDelete(int id)
        {
            if (!UserId.HasValue) return;
            var request = new NotificationRequest.RemoveFromUser
            {
                UserId = UserId.Value,
                NotificationId = id
            };
            var result = await NotificationService.DeleteUserNotificationAsync(request);
            if (!result.IsSuccess) {
                Snackbar.Add(@Loc["DeleteError"], Severity.Error);
                return;
            }
            Snackbar.Add(Loc["NotificationDeleted"].Value, Severity.Success);
            await LoadNotificationsAsync();
        }

        private async Task GetUserUnreadCountAsync()
        {
            if (!UserId.HasValue) return;
            var request = new NotificationRequest.GetForUser
            {
                UserId = UserId.Value
            };
            var result = await NotificationService.GetUserUnreadCountAsync(request);
            UnreadCount = result.IsSuccess ? result.Value?.Count ?? 0 : 0;
            StateHasChanged();
        }

        protected async Task OnPageChanged(int newPage)
        {
            if (_currentPage == newPage) return;

            _currentPage = newPage;
            await LoadNotificationsAsync();
        }
    }
}