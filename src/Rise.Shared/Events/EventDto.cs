namespace Rise.Shared.Events;

/// <summary>
/// Contains data transfer objects (DTOs) used for event-related operations.
/// </summary>
public class EventDto
{
    /// <summary>
    /// Represents a minimal overview of an event (for listings or carousels).
    /// </summary>
    public class Index
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required EventTimeSlotDto.Index Date { get; set; }
        public required string ImageUrl { get; set; }
    }

    /// <summary>
    /// Represents detailed event information (for event detail pages).
    /// </summary>
    public class Detail : Index
    {
        public required string Subject { get; set; }
    }

    /// <summary>
    /// Represents detailed event information (for event detail page).
    /// </summary>
    public class DetailExtended : Detail
    {
        public required string Content { get; set; }
        public required AddressDto.Index Address { get; set; }
        public double? Price { get; set; }
        public string? RegisterLink { get; set; }
    }
}