namespace Rise.Shared.Resto;

/// <summary>
/// Represents the response structure for resto-related operations.
/// </summary>
public static partial class RestoResponse
{
    public class Index
    {
        public IEnumerable<RestoDto> Restos { get; set; } = [];
        public int TotalCount { get; set; }
    }

}

