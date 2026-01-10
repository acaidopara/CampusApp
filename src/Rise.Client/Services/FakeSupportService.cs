using Rise.Shared.Common;
using Rise.Shared.Support;

namespace Rise.Client.Services;

public class FakeSupportService : ISupportService
{
    private readonly List<SupportDto.Index> _supports = new()
    {
        new()
        {
            Id = 1,
            Name = "Rita",
            OpeningHours = new List<SupportDto.OpeningHour>
            {
                new() { Day = DayOfWeek.Tuesday, Open = "17:00", Close = "19:00" },
                new() { Day = DayOfWeek.Wednesday, Open = "12:30", Close = "13:30" },
                new() { Day = DayOfWeek.Thursday, Open = "17:00", Close = "19:00" },
            }
        }
    };
    
    public Task<Result<SupportResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx = default)
    {
        var response = new SupportResponse.Index
        {
            Supports = _supports,
            TotalCount = _supports.Count
        };
        return Task.FromResult(Result<SupportResponse.Index>.Success(response));
    }

    public Task<Result<SupportResponse.ByName>> GetByNameAsync(string name, CancellationToken ctx = default)
    {
        var support = _supports.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (support is null)
            return Task.FromResult(Result<SupportResponse.ByName>.NotFound("Service not found"));

        var response = new SupportResponse.ByName { Support = support };
        return Task.FromResult(Result<SupportResponse.ByName>.Success(response));
    }

}