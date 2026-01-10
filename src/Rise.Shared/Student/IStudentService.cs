namespace Rise.Shared.Student;

public interface IStudentService
{
    Task<Result> UpdateCampusPreference(StudentRequest.Campus request, CancellationToken ct = default);
    Task<Result> UpdatePushNotifPreferences(StudentRequest.Preference request, CancellationToken ct = default);
    Task<Result> UpdateColourPreference(StudentRequest.Colour request, CancellationToken ct = default);

}