using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Rise.Domain.Infrastructure;

namespace Rise.Domain.Entities;

public class Menu : Entity
{

    [NotMapped]
    public Dictionary<string, List<MenuItem>> _items = new Dictionary<string, List<MenuItem>> {
        { "Ma", new List<MenuItem>() },
        { "Di", new List<MenuItem>() },
        { "Wo", new List<MenuItem>() },
        { "Do", new List<MenuItem>() },
        { "Vr", new List<MenuItem>() },
    };
    [NotMapped]
    public Dictionary<string, List<MenuItem>> Items
    {
        get => _items;
        set => _items = value;
    }
    public void AddMenuToDay(string dayAbbreviation, List<MenuItem> list)
    {
        if (string.IsNullOrWhiteSpace(dayAbbreviation))
            throw new ArgumentException("Day abbreviation cannot be null or empty.", nameof(dayAbbreviation));

        dayAbbreviation = char.ToUpper(dayAbbreviation[0])
                        + dayAbbreviation.Substring(1, 1).ToLower();

        Items[dayAbbreviation] = list;
    }

    public string ItemsJson
    {
        get => JsonSerializer.Serialize(Items, new JsonSerializerOptions());
        set => Items = JsonSerializer.Deserialize<Dictionary<string, List<MenuItem>>>(value, new JsonSerializerOptions())
                 ?? new Dictionary<string, List<MenuItem>>();
    }

    public DateTime _startDate;
    public required DateTime StartDate
    {
        get => _startDate;
        set => _startDate = value;
    }

    public bool _hasMenu;
    public bool HasMenu
    {
        get => _hasMenu;
        set => _hasMenu = value;
    }

    public string? _descriptionMenu;
    public string? DescriptionMenu
    {
        get => _descriptionMenu;
        set => _descriptionMenu = Guard.Against.NullOrWhiteSpace(value);
    }

    public ICollection<Resto> Restos { get; set; } = new List<Resto>();

}