namespace Rise.Shared.Absences;

public static class AbsenceDto
{
    public class Index
    {
        public int Id { get; set; }
        public string TeacherName { get; set; } = "";
        public DateTime DateAndTime { get; set; }
    }
}