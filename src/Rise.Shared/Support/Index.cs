namespace Rise.Shared.Support;

/// <summary>
/// Represents the response structure for support-related operations.
/// </summary>
public static partial class SupportResponse
{
    public class Index
    {
        public required IEnumerable<SupportDto.Index> Supports { get; set; }
        public int TotalCount { get; set; }
    }

    public class ByName
    {
        public required SupportDto.Index Support { get; set; }
    }
}