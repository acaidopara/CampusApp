namespace Rise.Domain.Users;

public class Employee : User
{
    private string _employeenumber = String.Empty;
    
    public string Employeenumber
    {
        get => _employeenumber;
        set => _employeenumber = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _title = String.Empty;

    public string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
}