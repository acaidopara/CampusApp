using Ardalis.Result;
using FastEndpoints;
using MudBlazor.Extensions;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;
using DeadlinesApi = Rise.Server.Endpoints.Deadlines;
namespace Tests.Deadlines;

[Collection<TestCollection>]
public class DeadlinesTest(App app) : TestBase
{
    [Fact]
    public async Task Get_DeadlinesUnauthorized_Failed()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100
        };

        var (rsp, result) = await app.Client.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound); // Dit is nu eenmaal hoe de service geimplementeerd is, dus geen Unauthorized
        result.ShouldBeNull();

    }

    [Fact]
    public async Task Get_DeadlinesAuthorized_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(4);
        result.Value.Deadlines.Where(d => d.Title.Equals("RISE Project Proposal")).Count().ShouldBe(1);
        result.Value.Deadlines.Where(d => d.Title.Equals("RISE Project Proposal")).First().Description.Equals("Submit initial project proposal for RISE course");
        result.Value.Deadlines.Where(d => d.Title.Equals("RISE Project Proposal")).First().DueDate.Equals(new DateTime(2025, 11, 1, 23, 59, 0));
        result.Value.Deadlines.Where(d => d.Title.Equals("RISE Project Proposal")).First().Course?.Equals("Real-life Integrated Software Engineering");

    }

    [Fact]
    public async Task Get_DeadlinesNextYearWithNoResult_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            StartDate = DateTime.Now.AddYears(1).ToIsoDateString()
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(0);
    }

    [Fact]
    public async Task Get_DeadlinesNextYearWithOneResult_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            StartDate = "2026-05-01"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Description.Equals("Formulier invullen omtrent je onderzoeksvoorstel, hoofdvraag, deelvragen,...");
        result.Value.Deadlines.First().DueDate.Equals(new DateTime(2026, 05, 15, 12, 30, 0));
        result.Value.Deadlines.First().Course?.Equals("Bachelorproef");
        result.Value.Deadlines.First().Title.Equals("Indienen onderzoeksvoorstel");
    }

    [Fact]
    public async Task Get_DeadlinesThisYearWithOneResult_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            EndDate = "2025-11-02"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Description.Equals("Submit initial project proposal for RISE course");
        result.Value.Deadlines.First().DueDate.Equals(new DateTime(2025, 11, 01, 23, 59, 0));
        result.Value.Deadlines.First().Course?.Equals("Real-life Integrated Software Engineering");
        result.Value.Deadlines.First().Title.Equals("RISE Project Proposal");
    }

    [Fact]
    public async Task Search_DeadlinesWithTitle_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "RISE Project Proposal"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Description.Equals("Submit initial project proposal for RISE course");
        result.Value.Deadlines.First().DueDate.Equals(new DateTime(2025, 11, 01, 23, 59, 0));
        result.Value.Deadlines.First().Course?.Equals("Real-life Integrated Software Engineering");
        result.Value.Deadlines.First().Title.Equals("RISE Project Proposal");
    }

    [Fact]
    public async Task Search_DeadlinesWithDescription_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "Submit initial project proposal for RISE course"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Description.Equals("Submit initial project proposal for RISE course");
        result.Value.Deadlines.First().DueDate.Equals(new DateTime(2025, 11, 01, 23, 59, 0));
        result.Value.Deadlines.First().Course?.Equals("Real-life Integrated Software Engineering");
        result.Value.Deadlines.First().Title.Equals("RISE Project Proposal");
    }

    [Fact]
    public async Task Search_DeadlinesWithCourse_Succes()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "Real-life Integrated Software Engineering"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(3);
    }

    [Fact]
    public async Task Search_DeadlinesWithNonExistantCourse_EmptyResult()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "dit vak bestaat niet :)"
        };

        var (rsp, result) = await app.Student.GETAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();

        result.Value.Deadlines.Count().ShouldBe(0);
    }

    [Fact]
    public async Task Post_Deadlines_MethodNotAllowed()
    {
        var request = new DeadlineRequest.GetForStudent
        {
            Skip = 0,
            Take = 100,
        };

        var (rsp, result) = await app.Student.POSTAsync<DeadlinesApi.Index, DeadlineRequest.GetForStudent, Result<DeadlineResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

}