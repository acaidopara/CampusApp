namespace Rise.Shared.Absences;

public static partial class AbsenceResponse
{
    public class Index
    {
        public IEnumerable<AbsenceDto.Index> Absences { get; set; } = [];
        public int TotalCount { get; set; } 
    }
}