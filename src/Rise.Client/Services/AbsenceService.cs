using Rise.Client.Api;
using Rise.Shared.Absences;

namespace Rise.Client.Services;

public class AbsenceService(TransportProvider _transport) : IAbsenceService
{
public async Task<Result<AbsenceResponse.Index>> GetAbsencesForDay(
    AbsenceRequest.DayRequest request, 
    CancellationToken ctx = default)
    {
        var day = request.Day.ToString("yyyy-MM-dd");
        var result = await _transport.Current.GetAsync<AbsenceResponse.Index>(
            $"/api/absences/{day}",
            cancellationToken: ctx
        );

        return result!;
    }

}