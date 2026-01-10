using Rise.Shared.Absences;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Absences;

/// <summary>
/// List all absences for a given day.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="absenceService"></param>
public class GetAbsencesByDay(IAbsenceService absenceService) : Endpoint<AbsenceRequest.DayRequest, Result<AbsenceResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/absences/{Day}");
        Roles(AppRoles.Student);
    }

    public override Task<Result<AbsenceResponse.Index>> ExecuteAsync(AbsenceRequest.DayRequest request , CancellationToken ct)
    {
        return absenceService.GetAbsencesForDay(request, ct);
    }
}