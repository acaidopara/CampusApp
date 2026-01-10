using Rise.Shared.Events;

namespace Rise.Shared.Infrastructure;

/// <summary>
/// Contains data transfer objects (DTOs) used for campus-related operations.
/// </summary>
public static partial class CampusDto
{
    /// <summary>
    /// Represents a minimal overview of a campus (for listings).
    /// </summary>
    public class Index
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasResto { get; set; } = false;
        public required AddressDto.Index Address { get; set; }
    }

    /// <summary>
    /// Represents detail campus information (for campus detail page).
    /// </summary>
    public class Detail : Index
    {
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string MapsUrl { get; set; } = string.Empty;
        public string? TourUrl { get; set; }
        public required FacilityDto.Index Facilities { get; set; }
    }
}