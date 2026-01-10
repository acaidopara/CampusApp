using Rise.Domain.Education;
using Rise.Domain.Users;

namespace Rise.Domain.CampusLife;

public class Job : Entity
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _companyName = string.Empty;
    public string CompanyName
    {
        get => _companyName;
        set => _companyName = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private Address? _address;
    public Address? Address
    {
        get => _address;
        set => _address = Guard.Against.Null(value);
    }
    
    private string _websiteUrl = string.Empty;
    public string WebsiteUrl
    {
        get => _websiteUrl;
        set => _websiteUrl = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _imageUrl = string.Empty;
    public string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _jobCategory = default!;
    public required string JobCategory
    {
        get => _jobCategory;
        set => _jobCategory = Guard.Against.NullOrWhiteSpace(value, nameof(Shared.CampusLife.JobCategory));
    }

    private EmailAddress? _emailAddress;
    public EmailAddress? EmailAddress
    {
        get => _emailAddress;
        set => _emailAddress = value;
    }

    private DateTime _startDate = DateTime.Now;
    public DateTime StartDate
    {
        get => _startDate;
        set => _startDate = value;
    }

    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate
    {
        get => _endDate;
        set => _endDate = value;
    }

    private double _salary;
    public double Salary
    {
        get => _salary;
        set => _salary = value;
    }

}