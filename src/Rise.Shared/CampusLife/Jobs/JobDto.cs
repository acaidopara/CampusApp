using Rise.Shared.Events;

namespace Rise.Shared.CampusLife.Jobs;

public static partial class JobDto
{
    public class Index
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public required AddressDto.Index Address { get; set; }
        public string ImageUrl { get; set; } = "";
        public required string JobCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Salary { get; set; }
    }

    public class Detail : Index
    {
        public string Description { get; set; } = "";
        public string WebsiteUrl { get; set; } = "";
        public string EmailAddress { get; set; } = "";
    }
}