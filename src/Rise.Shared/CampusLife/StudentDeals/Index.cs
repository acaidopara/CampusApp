namespace Rise.Shared.CampusLife.StudentDeals;

public static partial class StudentDealResponse
{
    public class Index
    {
        public IEnumerable<StudentDealDto.Index> StudentDeals { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public class Detail
    {
        public required StudentDealDto.Detail StudentDeal { get; set; }
    }
}