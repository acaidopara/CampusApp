namespace Rise.Domain.Users;

public class EmailAddress : ValueObject
{
    public string? Value { get; }
    
    private EmailAddress() 
    {
        
    }
    
    public EmailAddress(string email)
    {
        Value = email.ToLower().Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        if (Value != null) yield return Value.ToLower();
    }
}