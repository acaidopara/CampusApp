using Ardalis.Result;
using MudBlazor.Extensions;
using Rise.Shared.Deadlines;

namespace Rise.Client.Pages.Deadlines;

public class FakeDeadlineService : IDeadlineService
{
    private readonly int _delayMs;

    public FakeDeadlineService(int delayMs = 0)
    {
        _delayMs = delayMs;
    }
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(DeadlineRequest.GetForStudent request,
        CancellationToken ctx = default)
    {
        if (_delayMs > 0)
        {
            await Task.Delay(_delayMs);
        }
        
        var weekStart = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
        var deadlines = Enumerable.Range(1, 5)
            .Select(i => new DeadlineDto.Index
            {
                Id = i,
                Title = $"Deadline {i}",
                Description = $"Description {i}",
                Course = i % 2 == 0 ? "RISE" : "Bussines Analyse",
                DueDate = weekStart.AddDays(i),
                IsCompleted = false
            });

        var wrapper = new DeadlineResponse.Index
        {
            Deadlines = deadlines,
            TotalCount = 5
        };

        return await Task.FromResult(Result.Success(wrapper));
    }
}