using System.ComponentModel.DataAnnotations.Schema;
using Rise.Domain.Education;
using Rise.Domain.Events;

namespace Rise.Domain.Events;
public class Event : Entity
{
    private string _title = default!;
    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value, nameof(Title));
    }

    private EventTimeSlot _date = default!;
    public required EventTimeSlot Date
    {
        get => _date;
        set => _date = Guard.Against.Null(value, nameof(Date));
    }

    private string _imageUrl = "image1.jpg";

    public string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = Guard.Against.NullOrWhiteSpace(value, nameof(ImageUrl)) ?? _imageUrl;
    }


    private string _subject = default!;
    public required string Subject
    {
        get => _subject;
        set => _subject = Guard.Against.NullOrWhiteSpace(value, nameof(Subject));
    }

    [Column(TypeName = "TEXT")]
    private string _content = default!;
    public required string Content
    {
        get => _content;
        set => _content = Guard.Against.NullOrWhiteSpace(value, nameof(Content));
    }

    private Address _address = default!;
    public required Address Address
    {
        get => _address;
        set => _address = Guard.Against.Null(value, nameof(Address));
    }

    public double? Price { get; set; }
    public string? RegisterLink { get; set; }
}