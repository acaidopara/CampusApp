using Rise.Domain.Infrastructure;

namespace Rise.Domain.Entities;

public class MenuItem : Entity
{
    public String _name = String.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    public String _description = String.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    public bool _isVeganAndHalal = false;
    public required bool IsVeganAndHalal
    {
        get => _isVeganAndHalal;
        set => _isVeganAndHalal = value;
    }
    
    public bool _isVeggieAndHalal = false;
    public required bool IsVeggieAndHalal
    {
        get => _isVeggieAndHalal;
        set => _isVeggieAndHalal = value;
    }

    public FoodCategory _category = FoodCategory.Onbekend;
    public required FoodCategory Category
    {
        get => _category;
        set => _category = value;
    }
}