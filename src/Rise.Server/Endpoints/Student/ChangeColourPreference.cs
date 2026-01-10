using Rise.Shared.Identity;
using Rise.Shared.Student;

namespace Rise.Server.Endpoints.Student;
    
public class ChangeColourPreference( IStudentService studentService )
    : Endpoint<StudentRequest.Colour, Result>
{
    public override void Configure()
    {
        Put("/api/users/{UserId}/colour");
        Roles(AppRoles.Student);
    }

    public override async Task<Result> ExecuteAsync(StudentRequest.Colour req, CancellationToken ct)
    {
        return await studentService.UpdateColourPreference(req, ct);
    }
}