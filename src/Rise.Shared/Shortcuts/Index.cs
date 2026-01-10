namespace Rise.Shared.Shortcuts;

public static partial class ShortcutResponse
{
    public class Index
    {
        public IEnumerable<ShortcutDto.Index> Shortcuts { get; set; } = [];
        public int TotalCount { get; set; }
    }
}