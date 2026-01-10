using Rise.Shared.Common;

namespace Rise.Shared.Infrastructure;

public static partial class ClassroomDto
{
    public class Index
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public required BuildingDto.Index Building { get; set; } 
    }
}

public static partial class ClassroomRequest
{
    public class GetById : QueryRequest.SkipTake
    {
        public int CampusId { get; set; }
        public int BuildingId { get; set; }
        public int ClassroomId { get; set; }
    
        
        public class Validator : AbstractValidator<GetById>
        {
            public Validator()
            {
                RuleFor(x => x.CampusId).NotEmpty();
                RuleFor(x => x.BuildingId).NotEmpty();
                RuleFor(x => x.ClassroomId).NotEmpty();
            }
        }
    }
}