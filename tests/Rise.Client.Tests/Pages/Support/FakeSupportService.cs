using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Support;

namespace Rise.Client.Pages.Support
{
    internal class FakeSupportService : ISupportService
    {
        private readonly int _delayMs;
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

        public FakeSupportService(int delayMs = 0)
        {
            _delayMs = delayMs;
        }

        public async Task<Result<SupportResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request,
            CancellationToken ctx = default)
        {
            if (_delayMs > 0)
                await Task.Delay(_delayMs, ctx);

            var response = new SupportResponse.Index
            {
                Supports = _supports,
                TotalCount = _supports.Count
            };

            return Result<SupportResponse.Index>.Success(response);
        }

        public async Task<Result<SupportResponse.ByName>> GetByNameAsync(string name, CancellationToken ctx = default)
        {
            if (_delayMs > 0)
                await Task.Delay(_delayMs, ctx);

            var support = _supports.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (support is null)
                return Result<SupportResponse.ByName>.NotFound("Service not found");

            var response = new SupportResponse.ByName { Support = support };
            return Result<SupportResponse.ByName>.Success(response);
        }
    }
}
