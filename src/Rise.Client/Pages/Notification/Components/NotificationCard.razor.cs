using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Notifications;

namespace Rise.Client.Pages.Notification.Components;

public partial class NotificationCard
{

    [Parameter] public int NotificationId { get; set; }
    
    [SupplyParameterFromQuery(Name = "tab")]
    public string? Tab { get; set; }

    private NotificationDto.Detail? _notification;
    private int? _userId;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userIdString = user.FindFirst("Id")?.Value;

        if (int.TryParse(userIdString, out var parsedId))
            _userId = parsedId;

        var result = await NotificationService.GetNotificationByIdAsync(new GetByIdRequest.GetById
        {
            Id = NotificationId
        });
        _notification = result.IsSuccess ? result.Value.Notification : null;

        if (_notification != null && _userId.HasValue)
        {
            var readRequest = new NotificationRequest.ChangeRead
            {
                UserId = _userId.Value,
                NotificationId = NotificationId
            };
            await NotificationService.UpdateUserNotificationIsReadAsync(readRequest);
        }
    }
}
