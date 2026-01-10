namespace Rise.Shared.CampusLife.Jobs;

public static partial class JobResponse
{
    public class Index
    {
        public IEnumerable<JobDto.Index> Jobs { get; set; } = [];
        public int TotalCount { get; set; } 
    }
    
    public class Detail
    {
        public required JobDto.Detail Job { get; set; }
    }
}