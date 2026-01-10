using Rise.Shared.Common;

namespace Rise.Shared.Lessons;

/// <summary>
/// Provides methods for managing lesson-related operations.
/// </summary>
public interface ILessonService
{
    Task<Result<LessonResponse.Index>> GetIndexAsync(LessonRequest.Week request, CancellationToken ctx = default);
    Task<Result<LessonResponse.NextLesson>> GetNextLessonAsync(CancellationToken ctx = default);
}