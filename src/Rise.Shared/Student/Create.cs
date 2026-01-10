using Rise.Shared.Common;

namespace Rise.Shared.Student;

public static class StudentRequest 
{
    public class Campus : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public string CampusName { get; set; } = string.Empty;

        public class Validator : AbstractValidator<Campus>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();

                RuleFor(x => x.CampusName).NotEmpty();
            }
        }
    }

    public class Preference : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public bool LessonChangesEnabled { get; set; }
        public bool AbsenteesEnabled { get; set; }
        public bool NewsAndEventsEnabled { get; set; }
        public bool DeadlinesEnabled { get; set; }

        public class Validator : AbstractValidator<Preference>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
            }
        }
    }

    
    public class Colour : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public string ColourHex { get; set; } = "#FABC32";

        public class Validator : AbstractValidator<Colour>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();

                RuleFor(x => x.ColourHex).NotEmpty();
            }
        }
    }
}