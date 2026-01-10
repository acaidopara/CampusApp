using Rise.Shared.Common;

namespace Rise.Shared.Shortcuts;

public static class ShortcutRequest
{
    public class GetForUser : QueryRequest.SkipTake
    {
        public int UserId { get; set; }

        public class Validator : AbstractValidator<GetForUser>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
            }
        }
    }
    
    public class AddToUser : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public int ShortcutId { get; set; }

        public class Validator : AbstractValidator<AddToUser>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.ShortcutId).NotEmpty();
            }
        }
    }
    public class RemoveFromUser : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public int ShortcutId { get; set; }

        public class Validator : AbstractValidator<RemoveFromUser>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.ShortcutId).NotEmpty();
            }
        }
    }
    
    public class ChangeOrder : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public List<int> OrderedShortcutIds { get; set; } = new();

        public class Validator : AbstractValidator<ChangeOrder>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();

                RuleFor(x => x.OrderedShortcutIds)
                    .NotEmpty();
                RuleForEach(x => x.OrderedShortcutIds).NotEmpty();
            }
        }
    }

    public class UpdateColour : QueryRequest.SkipTake
    {
        public int UserId { get; set; }
        public int ShortcutId { get; set; }
        public required string Colour { get; set; }

        public class Validator : AbstractValidator<UpdateColour>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.ShortcutId).NotEmpty();
                RuleFor(x => x.Colour).NotEmpty();
            }
        }
    }
}