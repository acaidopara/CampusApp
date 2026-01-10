namespace Rise.Shared.Events;

public static class EventTimeSlotDto
{
    public class Index
    {
        public required DateOnly Date { get; set; }
        public required TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}