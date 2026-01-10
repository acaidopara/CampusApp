using Rise.Shared.Student;
using Rise.Client.Api;

namespace Rise.Client.Pages.Settings.Service;

public class StudentService(TransportProvider _transport) : IStudentService
{
    public async Task<Result> UpdateCampusPreference(StudentRequest.Campus request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PutAsync<StudentRequest.Campus>($"/api/users/{request.UserId}/campus", request, ctx))!;
    }

    public async Task<Result> UpdateColourPreference(StudentRequest.Colour request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PutAsync<StudentRequest.Colour>($"/api/users/{request.UserId}/colour", request, ctx))!;
    }

    public async Task<Result> UpdatePushNotifPreferences(StudentRequest.Preference request, CancellationToken ctx = default)
    {
        return (await _transport.Current.PutAsync<StudentRequest.Preference>($"/api/users/{request.UserId}/preferences",
            request, ctx))!;
    }
}