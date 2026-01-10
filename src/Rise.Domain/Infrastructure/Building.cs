using Rise.Domain.Education;

namespace Rise.Domain.Infrastructure;

public class Building : Entity
{
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
        set => _imageUrl = value; // Optional: Guard.Against.InvalidUrl(value) if you add a URL guard extension
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _description = string.Empty; // Added for consistency with UI/Campus
    public string Description
    {
        get => _description;
        set => _description = value; // Allow empty if no description
    }

    private Campus _campus = null!;
    public Campus Campus
    {
        get => _campus;
        set => _campus = Guard.Against.Null(value);
    }

    private List<Resto>? _restos = new();

    public List<Resto>? Restos
    {
        get => _restos;
        set => _restos = value;
    }

    private List<Classroom>? _classrooms = new();

    public List<Classroom>? Classrooms
    {
        get => _classrooms;
        set => _classrooms = value;
    }
    
    private CampusFacilities _facilities = null!;
    public CampusFacilities Facilities
    {
        get => _facilities;
        set => _facilities = Guard.Against.Null(value);
    }
    
    public void AddClassroom(Classroom classroom)
    {
        Guard.Against.Null(classroom);
        if (_classrooms != null && _classrooms.Any(b => b.Id == classroom.Id)) return;
        _classrooms?.Add(classroom);
        classroom.Building = this;
    }

    public void AddResto(Resto resto)
    {
        Guard.Against.Null(resto);
        if (_restos != null && _restos.Any(r => r.Id == resto.Id)) return;
        _restos?.Add(resto);
        resto.Building = this;
    }

}