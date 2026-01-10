using Rise.Domain.Education;
using Rise.Domain.Events;

namespace Rise.Domain.Infrastructure;

public class Campus : Entity
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = value; // Allow empty if no description
    }

    private Address _address = null!;
    public Address Address
    {
        get => _address;
        set => _address = Guard.Against.Null(value);
    }

    private string _imageUrl = string.Empty;
    public string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = value;
    }
    
    private CampusFacilities _facilities = null!;
    public CampusFacilities Facilities
    {
        get => _facilities;
        set => _facilities = Guard.Against.Null(value);
    }
    
    private string? _tourUrl;
    public string? TourUrl
    {
        get => _tourUrl;
        set => _tourUrl = value;
    }
    
    private string _mapsUrl = string.Empty;
    public string MapsUrl
    {
        get => _mapsUrl;
        set => _mapsUrl = value;
    }
    
    private List<Emergency> _emergencies = new();
    public IReadOnlyCollection<Emergency> Emergencies => _emergencies.AsReadOnly();

    private List<Event> _events = new();
    public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

    private List<Building> _buildings = new();

    public List<Building> Buildings
    {
        get => _buildings;
        set => _buildings = Buildings;
    }

    public void AddBuilding(Building building)
    {
        Guard.Against.Null(building);
        if (_buildings.Any(b => b.Id == building.Id)) return;
        _buildings.Add(building);
        building.Campus = this;
    }
}