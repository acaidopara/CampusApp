
namespace Rise.Shared.Resto;

/// <summary>
/// Contains data transfer objects (DTOs) used for resto-related operations.
/// </summary>
public  class MenuDto
{
    
        public required int Id { get; set; }
        public required Dictionary<String, List<MenuItemDto>> Items { get; set; }
        
        public required bool HasMenu { get; set; }
        public required string DescriptionMenu { get; set; }
    
}