using System.Net.Http.Json;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.Common;
using Rise.Client.Api;

namespace Rise.Client.Pages.CampusLife.Service
{
    public class StudentClubService(TransportProvider _transport) : IStudentClubService
    {
        public async Task<Result<StudentClubResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
           return (await _transport.Current.GetAsync<StudentClubResponse.Index>($"/api/studentclubs?{request.AsQuery()}", ctx))!;
        }
    }
}