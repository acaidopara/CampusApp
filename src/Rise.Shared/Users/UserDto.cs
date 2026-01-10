namespace Rise.Shared.Users;

/// <summary>
/// Contains data transfer objects (DTOs) used for department-related operations.
/// </summary>
public static class UserDto
{
    /// <summary>
    /// Represents a department index containing minimalistic department details.
    /// </summary>
    public class Index
    {
        public required int Id { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public required string Title { get; set; }
    }
}