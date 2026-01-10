namespace Rise.Shared.CampusLife.StudentClub;

public static partial class StudentClubDto
{
    public class Index
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string? WebsiteUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? ShieldImageUrl { get; set; }
        public string? Email { get; set; }
    }

}