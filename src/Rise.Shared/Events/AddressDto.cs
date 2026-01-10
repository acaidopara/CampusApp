namespace Rise.Shared.Events;

public static class AddressDto
{
    public class Index
    {
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
    }
}