using Rise.Shared.Common;

namespace Rise.Shared.Notifications
{
    public static class NotificationRequest
    {
        public class GetForUser : QueryRequest.SkipTake
        {
            public int UserId { get; init; }

            public string? TopicTerm { get; set; }

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
            public int NotificationId { get; set; }

            public class Validator : AbstractValidator<AddToUser>
            {
                public Validator()
                {
                    RuleFor(x => x.UserId).NotEmpty();
                    RuleFor(x => x.NotificationId).NotEmpty();
                }
            }
        }
        public class RemoveFromUser : QueryRequest.SkipTake
        {
            public int UserId { get; init; }
            public int NotificationId { get; init; }

            public class Validator : AbstractValidator<RemoveFromUser>
            {
                public Validator()
                {
                    RuleFor(x => x.UserId).NotEmpty();
                    RuleFor(x => x.NotificationId).NotEmpty();
                }
            }
        }

        public class ChangeRead : QueryRequest.SkipTake
        {
            public int UserId { get; set; }
            public int NotificationId { get; set; }

            public class Validator : AbstractValidator<ChangeRead>
            {
                public Validator()
                {
                    RuleFor(x => x.UserId).NotEmpty();
                    RuleFor(x => x.NotificationId).NotEmpty();
                }
            }
        }
    }
}
