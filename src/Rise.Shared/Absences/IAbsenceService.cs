namespace Rise.Shared.Absences;

public interface IAbsenceService
{
    Task<Result<AbsenceResponse.Index>> GetAbsencesForDay(AbsenceRequest.DayRequest request, CancellationToken ctx = default);
}