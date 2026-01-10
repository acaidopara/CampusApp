using Rise.Shared.Student;

namespace Rise.Server.Endpoints.Student;
    
public class ChangeCampusPreference( IStudentService studentService )
    : Endpoint<StudentRequest.Campus, Result>
{
    public override void Configure()
    {
        Put("/api/users/{UserId}/campus");
        AllowAnonymous();
    }

    public override async Task<Result> ExecuteAsync(StudentRequest.Campus req, CancellationToken ct)
    {
        return await studentService.UpdateCampusPreference(req, ct);
    }
}