namespace Rise.Shared.CampusLife.StudentDeals;

public static partial class StudentDealDto
{
    public class Index
    {
        public int Id { get; set; }
        public string Store { get; set; } = "";
        public string Name { get; set; } = "";
        public int? Discount { get; set; }
        public DateTime DueDate { get; set; }
        public required string PromoCategory { get; set; }
        public string ImageUrl { get; set; } = "";
    }
    
    public class Detail : Index
    {
        public string Description { get; set; } = "";
        public string WebUrl { get; set; } = "";
        public string? DiscountCode { get; set; }
    }
}