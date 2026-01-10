using Rise.Shared.Common;

namespace Rise.Shared.Infrastructure;

public static partial class BuildingRequest
{
    public class GetById : QueryRequest.SkipTake
    {
        public int CampusId { get; set; }
        public int BuildingId { get; set; }
    
        
        public class Validator : AbstractValidator<GetById>
        {
            public Validator()
            {
                RuleFor(x => x.CampusId).NotEmpty();
                RuleFor(x => x.BuildingId).NotEmpty();
            }
        }
    }
}
