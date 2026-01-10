namespace Rise.Shared.Support;

public static class SupportDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<OpeningHour> OpeningHours { get; set; }
    }
    
    public class OpeningHour
    {
        public required DayOfWeek Day { get; set; }
        public required string Open { get; set; }
        public required string Close { get; set; }
    }
}