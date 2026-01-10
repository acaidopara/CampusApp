namespace Rise.Shared.Identity.Roles;

/// <summary>
/// Represents a request model used for role-related operations.
/// </summary>
public static partial class RoleRequest
{
    /// <summary>
    /// Represents the model for creating a new role in the system.
    /// </summary>
    public class Create
    {
        /// <summary>
        /// The name of the role.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Provides validation logic for the RoleRequest.Create model.
        /// </summary>
        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
            }
        }
    }
}