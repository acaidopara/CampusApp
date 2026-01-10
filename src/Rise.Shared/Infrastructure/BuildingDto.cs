using Rise.Shared.Events;

namespace Rise.Shared.Infrastructure;

/// <summary>
/// Contains data transfer objects (DTOs) used for building-related operations.
/// </summary>
public static partial class BuildingDto
{
    public class Index
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required AddressDto.Index Address { get; set; }
        public int CampusId { get; set; }
        public string Campus { get; set; } = string.Empty;
    }
    
    public class Detail : Index
    {
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public required FacilityDto.Index Facilities { get; set; }
    }
}