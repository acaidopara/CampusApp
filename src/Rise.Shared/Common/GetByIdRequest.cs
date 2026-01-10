namespace Rise.Shared.Common;

/// <summary>
/// Provides structure to facilitate structured queries for data retrieval scenarios.
/// This includes handling a getById.
/// </summary>
public static partial class GetByIdRequest
{
    public class GetById
    {
        public int? Id { get; set; }
        
        public class Validator : AbstractValidator<GetById>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
    }
}