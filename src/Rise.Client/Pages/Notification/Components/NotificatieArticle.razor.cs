using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rise.Shared.Notifications;

namespace Rise.Client.Pages.Notification.Components
{
    public partial class NotificatieArticle
    {
        [Parameter] public NotificationDto.Index? Notification { get; set; }

        [Parameter]
        public EventCallback<int> OnToggleRead { get; set; }

        [Parameter]
        public EventCallback<int> OnDelete { get; set; }

        [Parameter] public required string Tab { get; set; }

        private double _startX;
        private double _translateX;
        private bool _isDragging;
        private bool _movedDuringDrag;
        private bool _isRemoving;
        private bool _isCollapsing;
        private const double MaxLeft = -320;
        private const double DeleteThreshold = -120;

        private string CardStyle =>
            $"transform: translateX({_translateX}px); transition: {(_isDragging ? "none" : "transform 220ms ease")}";

        private string DeleteAreaStyle
        {
            get
            {
                var width = _translateX < 0 ? Math.Min(-_translateX, Math.Abs(MaxLeft)) : 0;
                var transition = _isDragging ? "none" : "width 220ms ease";
                return $"width: {width}px; transition: {transition}; overflow: hidden; display: flex; align-items: center; justify-content: center;";
            }
        }

        private Task OnPointerDown(PointerEventArgs e)
        {
            if (_isRemoving || _isCollapsing) return Task.CompletedTask;
            _isDragging = true;
            _movedDuringDrag = false;
            _startX = e.ClientX;
            return Task.CompletedTask;
        }

        private Task OnPointerMove(PointerEventArgs e)
        {
            if (!_isDragging || _isRemoving || _isCollapsing)
                return Task.CompletedTask;

            var delta = e.ClientX - _startX;

            if (Math.Abs(delta) > 6)
                _movedDuringDrag = true;

            if (delta >= 0)
                _translateX = 0;
            else
                _translateX = Math.Max(delta, MaxLeft);

            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task OnPointerUp(PointerEventArgs e)
        {
            if (!_isDragging || _isRemoving || _isCollapsing)
                return;

            _isDragging = false;

            if (_translateX <= DeleteThreshold)
            {
                _isRemoving = true;
                StateHasChanged();
                await Task.Delay(10);

                _translateX = -1000;
                StateHasChanged();
                await Task.Delay(260);

                _isCollapsing = true;
                StateHasChanged();
                await Task.Delay(220);

                if (OnDelete.HasDelegate)
                    if (Notification != null)
                        await OnDelete.InvokeAsync(Notification.Id);
            }
            else
            {
                _translateX = 0;
                StateHasChanged();
                await Task.Delay(220);
            }
        }

        private Task OnPointerCancel(PointerEventArgs e)
        {
            _isDragging = false;
            _translateX = 0;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task ToggleRead()
        {
            if (OnToggleRead.HasDelegate)
                if (Notification != null)
                    await OnToggleRead.InvokeAsync(Notification.Id);
        }

        private async Task OnClick()
        {
            if (_movedDuringDrag || _isRemoving || _isCollapsing)
                return;

            if (Notification is { IsRead: false })
                await ToggleRead();

            if (Notification != null)
            {
                var tabQuery = string.IsNullOrEmpty(Tab) ? string.Empty : $"?tab={Uri.EscapeDataString(Tab)}";
                NavigationManager.NavigateTo($"/notificaties/notificatie/{Notification.Id}{tabQuery}");
            }
        }
    }
}
