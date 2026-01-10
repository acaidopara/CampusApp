namespace Rise.Shared.Departments;

/// <summary>
/// Represents a static utility class containing request-related structures for departments.
/// </summary>
public static partial class DepartmentRequest
{
    public class Create
    {
        /// <summary>
        /// The name of the department.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// A short description of the department.
        /// </summary>
        public string? Description { get; set; }
        
        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(250); 
                RuleFor(x => x.Description).NotEmpty().MaximumLength(4_000);
            }
        }
    }
}

public static partial  class DepartmentResponse
{
    public class Create
    {
        public int DepartmentId { get; set; }
    }
}