
namespace Rise.Domain.CampusLife;

public class StudentDeal : Entity
{
    private string _store = string.Empty;
    public string Store
    {
        get => _store;
        set => _store = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private int? _discount = 0;

    public int? Discount
    {
        get => _discount;
        set => _discount = value;
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private DateTime _dueDate = DateTime.Now;
    public DateTime DueDate
    {
        get => _dueDate;
        set => _dueDate = value;
    }

    private string _promoCategory = "Andere";
    public string PromoCategory
    {
        get => _promoCategory;
        set => _promoCategory = Guard.Against.NullOrWhiteSpace(value, nameof(PromoCategory));
    }
    
    private string _webUrl = string.Empty;
    public string WebUrl
    {
        get => _webUrl;
        set => _webUrl = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _imageUrl = string.Empty;
    public string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string? _discountCode = string.Empty;
    public string? DiscountCode
    {
        get => _discountCode;
        set => _discountCode = Guard.Against.NullOrWhiteSpace(value);
    }
}