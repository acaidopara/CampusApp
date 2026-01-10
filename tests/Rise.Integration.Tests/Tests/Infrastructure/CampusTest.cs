using Ardalis.Result;
using FastEndpoints;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;

namespace Tests.Infrastructure;
[Collection<TestCollection>]
public class CampusTest(App app) : TestBase
{
    [Fact]
    public async Task Get_Campuses_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var (rsp, result) = await app.Client.GETAsync<CampusesIndex, QueryRequest.SkipTake, Result<CampusResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Campuses.ShouldNotBeEmpty();
        result.Value.Campuses.ToList().Count.ShouldBe(5);

        result.Value.Campuses.Where(c => c.Name.Equals("Campus Schoonmeersen")).Count().ShouldBe(1);
        result.Value.Campuses.Where(c => c.Name.Equals("Campus Schoonmeersen")).First().Address.AddressLine1.ShouldBeEquivalentTo("Valentin Vaerwyckweg");
        result.Value.Campuses.Where(c => c.Name.Equals("Campus Schoonmeersen")).First().HasResto.ShouldBeTrue();

        result.Value.Campuses.Where(c => c.Name.Equals("Campus Aalst")).First().HasResto.ShouldBeFalse();
    }

    [Fact]
    public async Task Post_Campuses_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var (rsp, result) = await app.Client.POSTAsync<CampusesIndex, QueryRequest.SkipTake, Result<CampusResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();

    }

    [Fact]
    public async Task Search_CampusMercator_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Campus Mercator"
        };

        var (rsp, result) = await app.Client.GETAsync<CampusesIndex, QueryRequest.SkipTake, Result<CampusResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Campuses.ShouldNotBeEmpty();
        result.Value.Campuses.ToList().Count.ShouldBe(1);

        result.Value.Campuses.Count().ShouldBe(1);
        result.Value.Campuses.First().Address.AddressLine1.ShouldBeEquivalentTo("Henleykaai");
        result.Value.Campuses.First().Name.ShouldBeEquivalentTo("Campus Mercator");
        result.Value.Campuses.First().HasResto.ShouldBeTrue();
    }

    [Fact]
    public async Task Search_NonExistantCampus_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Campus Niet bestaand"
        };

        var (rsp, result) = await app.Client.GETAsync<CampusesIndex, QueryRequest.SkipTake, Result<CampusResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Value.Campuses.ToList().ShouldBeEmpty();
    }

    [Fact]
    public async Task Get_CampusById1_Success()
    {
        var request = new GetByIdRequest.GetById
        {
            Id = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetCampusById, GetByIdRequest.GetById, Result<CampusResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Campus.ShouldNotBeNull();
        result.Value.Campus.Name.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Campus.ImageUrl.ShouldBeEquivalentTo("Schoonmeersen.jpg");
        result.Value.Campus.MapsUrl.ShouldBeEquivalentTo("https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Schoonmeersen+Valentin+Vaerwyckweg+1+Gent&travelmode=transit");
        result.Value.Campus.TourUrl?.ShouldBeEquivalentTo("https://youreka-virtualtours.be/tours/hogent_schoonmeersen/");
        result.Value.Campus.Facilities.Library.ShouldBeTrue();
        result.Value.Campus.Facilities.BikeStorage.ShouldBeTrue();
        result.Value.Campus.HasResto.ShouldBeTrue();
    }

    [Fact]
    public async Task Get_CampusById100_NotFound()
    {
        var request = new GetByIdRequest.GetById
        {
            Id = 100
        };

        var (rsp, result) = await app.Client.GETAsync<GetCampusById, GetByIdRequest.GetById, Result<CampusResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();

        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Post_CampusById1_Failed()
    {
        var request = new GetByIdRequest.GetById
        {
            Id = 1
        };

        var (rsp, result) = await app.Client.POSTAsync<GetCampusById, GetByIdRequest.GetById, Result<CampusResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

}