using Rise.Domain.Users;

namespace Rise.Domain.CampusLife;

public class StudentClub : Entity
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
        set => _description = value;
    }
    
    private string? _websiteUrl = string.Empty;
    public string? WebsiteUrl
    {
        get => _websiteUrl;
        set => _websiteUrl = value;
    }
    
    private string? _facebookUrl = string.Empty;
    public string? FacebookUrl
    {
        get => _facebookUrl;
        set => _facebookUrl = value;
    }

    private string? _instagramUrl = string.Empty;
    public string? InstagramUrl
    {
        get => _instagramUrl;
        set => _instagramUrl = value;
    }
    
    private string? _shieldImageUrl = string.Empty;
    public string? ShieldImageUrl
    {
        get => _shieldImageUrl;
        set => _shieldImageUrl = value;
    }

    private EmailAddress? _email;
    public EmailAddress? Email
    {
        get => _email;
        set => _email = value;
    }
    
}