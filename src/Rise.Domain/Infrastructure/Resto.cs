using Rise.Domain.Entities;
namespace Rise.Domain.Infrastructure;

public class Resto : Entity
{
    private Menu? _menu;
    public Menu? Menu
    {
        get => _menu;
        set => _menu = value;
    }
    private string? _coordinates = "";
    public string? Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }
    private String _name = "";
    public String Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }
    private Building? _building;
    public Building? Building
    {
        get => _building;
        set => _building = value;
    }
 

}