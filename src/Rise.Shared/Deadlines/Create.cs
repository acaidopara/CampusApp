using FluentValidation;
using Rise.Shared.Common;

namespace Rise.Shared.Deadlines;

/// <summary>
/// Static utility class containing request models for deadline operations.
/// These models define the structure of incoming requests for fetching and updating deadlines.
/// Includes validation rules to ensure data integrity.
/// </summary>
public static partial class DeadlineRequest
{
    /// <summary>
    /// Request model for fetching a paginated index of deadlines for a student.
    /// Inherits from SkipTake for pagination support.
    /// </summary>
    public class GetForStudent : QueryRequest.SkipTake
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        
        /// <summary>
        /// Validator for the GetForStudent request model.
        /// Ensures pagination parameters are valid.
        /// </summary>
        public class Validator : AbstractValidator<GetForStudent>
        {
            public Validator()
            {
                RuleFor(x => x.Skip).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(1000);
                When(x => !string.IsNullOrWhiteSpace(x.StartDate), () =>
                {
                    RuleFor(x => x.StartDate)
                        .Matches(@"\d{4}-\d{2}-\d{2}")
                        .WithMessage("StartDate must be in yyyy-MM-dd format.");
                });
                When(x => !string.IsNullOrWhiteSpace(x.EndDate), () =>
                {
                    RuleFor(x => x.EndDate)
                        .Matches(@"\d{4}-\d{2}-\d{2}")
                        .WithMessage("EndDate must be in yyyy-MM-dd format.");
                });
            }
        }
    }

    /// <summary>
    /// Request model for toggling the completion status of a deadline.
    /// </summary>
    public class ToggleCompletion
    {
        /// <summary>
        /// The ID of the deadline to update.
        /// </summary>
        public int DeadlineId { get; set; }
        
        /// <summary>
        /// The new completion status (true for completed, false for incomplete).
        /// </summary>
        public bool IsCompleted { get; set; }
        
        /// <summary>
        /// Validator for the ToggleCompletion request model.
        /// Ensures the deadline ID is provided.
        /// </summary>
        public class Validator : AbstractValidator<ToggleCompletion>
        {
            public Validator()
            {
                RuleFor(x => x.DeadlineId).NotEmpty();
            }
        }
    }
}