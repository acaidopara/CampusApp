using Rise.Shared.Support;

namespace Rise.Client.Pages.Support.Content;

public partial class SupportPage
{
    protected bool _isLoading = true;
    private SupportDto.Index? _support;

    protected override async Task OnInitializedAsync()
    {
        var result = await SupportService.GetByNameAsync("Rita");
        if (result.IsSuccess)
            _support = result.Value.Support;
        _isLoading = false;
    }

    private bool IsOpen()
    {
        if (_support == null) return false;

        var brusselsTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Brussels");
        var todayHours = _support.OpeningHours.FirstOrDefault(h => h.Day == brusselsTime.DayOfWeek);
        if (todayHours == null) return false;

        var now = brusselsTime.TimeOfDay;
        var openTime = TimeSpan.Parse(todayHours.Open);
        var closeTime = TimeSpan.Parse(todayHours.Close);

        return now >= openTime && now <= closeTime;
    }
}