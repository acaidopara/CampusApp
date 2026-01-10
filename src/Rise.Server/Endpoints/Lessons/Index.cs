using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Lessons;

namespace Rise.Server.Endpoints.Lessons;


/// <summary>
/// List all Lessons.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="lessonService"></param>
public class Index(ILessonService lessonService) : Endpoint<LessonRequest.Week, Result<LessonResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/lessons");
        Roles(AppRoles.Student);
    }

    public override Task<Result<LessonResponse.Index>> ExecuteAsync(LessonRequest.Week req, CancellationToken ct)
    {
        return lessonService.GetIndexAsync(req, ct);
    }
}