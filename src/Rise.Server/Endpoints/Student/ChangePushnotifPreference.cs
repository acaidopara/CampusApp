using Rise.Shared.Identity;
using Rise.Shared.Student;

namespace Rise.Server.Endpoints.Student;
    
public class ChangePushnotifPreference( IStudentService studentService )
    : Endpoint<StudentRequest.Preference, Result>
{
    public override void Configure()
    {
        Put("/api/users/{UserId}/preferences");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(StudentRequest.Preference req, CancellationToken ct)
    {
        return await studentService.UpdatePushNotifPreferences(req, ct);
    }
}