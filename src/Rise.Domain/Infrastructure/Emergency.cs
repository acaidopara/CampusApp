namespace Rise.Domain.Infrastructure;

public class Emergency : Entity
{
    private string _title = string.Empty;
    public required string Title
    {
        get => _title;
        set => _title = value;
    }

    private string _message = string.Empty;
    public required string Message
    {
        get => _message;
        set => _message = value;
    }

    public bool IsActive { get; set; }

    public DateTime DateAndTime { get; set; }

    private Campus? _campus;
    public Campus? Campus
    {
        get => _campus;
        set => _campus = value;
    }
}