namespace Rise.Shared.CampusLife.StudentClub;

public static partial class StudentClubResponse
{
    public class Index
    {
        public IEnumerable<StudentClubDto.Index> StudentClubs { get; set; } = [];
        public int TotalCount { get; set; }
    }
}