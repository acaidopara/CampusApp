namespace Rise.Shared.Lessons;

public static partial class LessonRequest
{
    public class Week
    {
        public DateTime StartDate { get ; set; }
        public DateTime EndDate { get ; set; }

        public string AsQuery()
        {
            var start = Uri.EscapeDataString(StartDate.ToString("o"));
            var end = Uri.EscapeDataString(EndDate.ToString("o"));
            return $"startDate={start}&endDate={end}";
        }
    }
}