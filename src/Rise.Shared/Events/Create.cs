using Rise.Shared.Common;
using Rise.Shared.News;
using Rise.Shared.CampusLife;

namespace Rise.Shared.Events;
public static partial class TopicRequest
{
    public class GetBasedOnTopic : QueryRequest.SkipTake
    {
        public string? Topic { get; set; } = Topics.All.Name;
        
        public new string AsQuery() => base.AsQuery() + $"&topic={Topic}";
        
        public class Validator : AbstractValidator<GetBasedOnTopic>
        {
            private bool BeAValidTopic(string? topic)
                => Topics.AllTopics.Any(t => t.Name == topic);
            
            public Validator()
            {
                RuleFor(x => x.Topic)
                    .NotEmpty()
                    .Must(BeAValidTopic)
                    .WithMessage("Invalid topic specified.");
            }
        }
    }
    
    public class GetBasedOnJobCategory : QueryRequest.SkipTake
    {
        public string? JobCategory { get; set; } = CategoriesJob.All.Name;
        
        public new string AsQuery() => base.AsQuery() + $"&jobcategory={JobCategory}";
        
        public class Validator : AbstractValidator<GetBasedOnJobCategory>
        {
            private bool BeAValidTopic(string? jobcategory)
                => CategoriesJob.AllJobs.Any(j => j.Name == jobcategory);
            
            public Validator()
            {
                RuleFor(x => x.JobCategory)
                    .NotEmpty()
                    .Must(BeAValidTopic)
                    .WithMessage("Invalid jobcategory specified.");
            }
        }
    }
    
    public class GetBasedOnPromoCategory : QueryRequest.SkipTake
    {
        public string? PromoCategory { get; set; } = CategoriesPromo.Other.Name;
        
        public new string AsQuery() => base.AsQuery() + $"&promocategory={PromoCategory}";
        
        public class Validator : AbstractValidator<GetBasedOnPromoCategory>
        {
            private bool BeAValidTopic(string? promocategory)
                => CategoriesPromo.AllCategories.Any(j => j.Name == promocategory);
            
            public Validator()
            {
                RuleFor(x => x.PromoCategory)
                    .NotEmpty()
                    .Must(BeAValidTopic)
                    .WithMessage("Invalid promocategory specified.");
            }
        }
    }
}