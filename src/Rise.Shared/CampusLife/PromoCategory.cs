namespace Rise.Shared.CampusLife;

/// <summary>
/// Represents a promo category with a name and optional Font Awesome icon.
/// </summary>
public record PromoCategory(string Name, string? Icon);

public static class CategoriesPromo
{
    public static readonly PromoCategory All = new("Alle", null);
    public static readonly PromoCategory Free = new("Gratis", "fa-solid fa-gift");
    public static readonly PromoCategory Study = new("Onderwijs", "fa-solid fa-book-bookmark");
    public static readonly PromoCategory Home = new("Thuis", "fa-solid fa-house");
    public static readonly PromoCategory Culture = new("Cultuur", "fa-solid fa-paintbrush");
    public static readonly PromoCategory Sport = new("Sport", "fa-regular fa-futbol");
    public static readonly PromoCategory Clothing = new("Kledij", "fa-solid fa-shirt");
    public static readonly PromoCategory Food = new("Eten", "fa-solid fa-utensils");
    public static readonly PromoCategory Tech = new("Tech", "fa-solid fa-desktop");
    public static readonly PromoCategory Other = new("Andere", null);

    public static readonly List<PromoCategory> AllCategories = new() { All, Free, Study, Home, Culture, Sport, Clothing, Food, Tech, Other };
}
