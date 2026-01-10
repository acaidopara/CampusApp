namespace Rise.Shared.Events;

/// <summary>
/// Represents the response structure for event-related operations.
/// </summary>
public static class EventResponse
{
    public class Index
    {
        public required IEnumerable<EventDto.Index> Events { get; init; }
    }

    public class Detail
    {
        public required IEnumerable<EventDto.Detail> Events { get; init; }
        public int TotalCount { get; init; }
    }

    public class DetailExtended
    {
        public EventDto.DetailExtended Event { get; init; } = null!;
    }
}