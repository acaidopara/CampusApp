using Rise.Shared.Lessons;
using Rise.Client.Api;
namespace Rise.Client.Pages.Timetable.Service;

public class LessonService(TransportProvider _transport) : ILessonService
{
    public async Task<Result<LessonResponse.Index>> GetIndexAsync(LessonRequest.Week request, CancellationToken ctx = default)
    {
        var result = await _transport.Current.GetAsync<LessonResponse.Index>($"/api/lessons?{request.AsQuery()}", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result<LessonResponse.NextLesson>> GetNextLessonAsync(CancellationToken ctx = default)
    {
        var result = await _transport.Current.GetAsync<LessonResponse.NextLesson>($"/api/lessons/next", cancellationToken: ctx);
        return result!;
    }
}
