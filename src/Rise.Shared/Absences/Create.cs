using Rise.Shared.Common;

namespace Rise.Shared.Absences;

public static class AbsenceRequest
{
    public class DayRequest : QueryRequest.SkipTake
    {
        public DateTime Day { get; set; }

        public class Validator : AbstractValidator<DayRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Day).NotEmpty();
            }
        }
    }
}