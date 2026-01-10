
using Rise.Domain.Entities;

namespace Rise.Shared.Resto;

/// <summary>
/// Contains data transfer objects (DTOs) used for resto-related operations.
/// </summary>
public  class MenuItemDto
{
        public required int Id { get; set; }
        public required String Name { get; set; }
        public required String Description { get; set; }
        public required bool IsVeganAndHalal { get; set; }
        public required bool IsVeggieAndHalal { get; set; }
        public required FoodCategory FoodCategory { get; set; }
    
}