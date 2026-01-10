using Rise.Shared.Users;

namespace Rise.Shared.Departments;

/// <summary>
/// Contains data transfer objects (DTOs) used for department-related operations.
/// </summary>
public static class DepartmentDto
{
    /// <summary>
    /// Represents a department index containing minimalistic department details.
    /// </summary>
    public class Index
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string DepartmentType {get; set;}
        public UserDto.Index? Manager { get; set; }
    }
    
}