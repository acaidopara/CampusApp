namespace Rise.Shared.Student;

public static partial class StudentDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string StudentNumber { get; set; }
        public required string PreferedCampus { get; set; }
    }
}