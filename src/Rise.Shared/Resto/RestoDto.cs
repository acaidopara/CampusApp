
namespace Rise.Shared.Resto;

/// <summary>
/// Contains data transfer objects (DTOs) used for resto-related operations.
/// </summary>
public class RestoDto
{
   
    
        public required int Id { get; set; }
        public required String Name { get; set; }
        public required String CampusName { get; set; }
        public required String Coordinates { get; set; }
        public required MenuDto Menu { get; set; }
    
    
}