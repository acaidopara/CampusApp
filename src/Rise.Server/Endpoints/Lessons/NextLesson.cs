using Rise.Shared.Identity;
using Rise.Shared.Lessons;

namespace Rise.Server.Endpoints.Lessons;

/// <summary>
/// List all Lessons.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="lessonService"></param>
public class NextLesson(ILessonService lessonService) : EndpointWithoutRequest<Result<LessonResponse.NextLesson>>
{
    public override void Configure()
    {
        Get("/api/lessons/next");
        Roles(AppRoles.Student);
    }

    public override Task<Result<LessonResponse.NextLesson>> ExecuteAsync(CancellationToken ct)
    {
        return lessonService.GetNextLessonAsync(ct);
    }
}