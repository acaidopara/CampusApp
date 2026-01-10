using Ardalis.Result;
using FastEndpoints;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;

namespace Tests.Infrastructure;

[Collection<TestCollection>]
public class BuildingTest(App app) : TestBase
{
    [Fact]
    public async Task Get_Buildings_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var (rsp, result) = await app.Client.GETAsync<BuildingsIndex, QueryRequest.SkipTake, Result<BuildingResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Buildings.ShouldNotBeEmpty();
        result.Value.Buildings.ToList().Count.ShouldBe(10);

        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw T")).Count().ShouldBe(1);
        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw T")).First().Address.AddressLine1.ShouldBeEquivalentTo("Voskenslaan");
        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw T")).First().Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw T")).First().Address.City.ShouldBeEquivalentTo("9000");

        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw B")).Count().ShouldBe(2);
    }

    [Fact]
    public async Task Post_Buildings_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var (rsp, result) = await app.Client.POSTAsync<BuildingsIndex, QueryRequest.SkipTake, Result<BuildingResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();

    }

    [Fact]
    public async Task Search_BuildingE_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Gebouw E"
        };

        var (rsp, result) = await app.Client.GETAsync<BuildingsIndex, QueryRequest.SkipTake, Result<BuildingResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Buildings.ShouldNotBeEmpty();
        result.Value.Buildings.ToList().Count.ShouldBe(1);

        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw E")).First().Address.AddressLine1.ShouldBeEquivalentTo("Valentin Vaerwyckweg");
        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw E")).First().Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Buildings.Where(b => b.Name.Equals("Gebouw E")).First().Address.City.ShouldBeEquivalentTo("9000");

    }

    [Fact]
    public async Task Search_NonExistantBuilding_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Building Niet bestaand"
        };

        var (rsp, result) = await app.Client.GETAsync<BuildingsIndex, QueryRequest.SkipTake, Result<BuildingResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Value.Buildings.ToList().ShouldBeEmpty();
    }

    [Fact]
    public async Task Get_BuildingById1_Success()
    {
        var request = new BuildingRequest.GetById
        {
            BuildingId = 1,
            CampusId = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetBuildingById, BuildingRequest.GetById, Result<BuildingResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Building.ShouldNotBeNull();
        result.Value.Building.Name.ShouldBeEquivalentTo("Gebouw B");
        result.Value.Building.ImageUrl.ShouldBeEquivalentTo("GebouwB.jpg");
        result.Value.Building.Description.ShouldBeEquivalentTo("Gebouw B is het oorspronkelijke theorie‑ en onderwijsgebouw op Campus Schoonmeersen. Het gebouw huisvest vooral grote leslokalen, auditoria en studentenzones zoals een cafetaria en onthaal. Vanaf hier kunnen studenten makkelijk wandelen naar de andere gebouwen op de campus, en je vindt er ook het onthaal voor bezoekers.");
        result.Value.Building.Facilities.Library.ShouldBeFalse();
        result.Value.Building.Facilities.RevolteRoom.ShouldBeFalse();
        result.Value.Building.Facilities.BikeStorage.ShouldBeTrue();
        result.Value.Building.Facilities.Restaurant.ShouldBeTrue();
        result.Value.Building.Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Building.CampusId.ShouldBeEquivalentTo(1);
    }

    [Fact]
    public async Task Get_BuildingById100_NotFound()
    {
        var request = new BuildingRequest.GetById
        {
            BuildingId = 100,
            CampusId = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetBuildingById, BuildingRequest.GetById, Result<BuildingResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();

        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Post_BuildingById1_Failed()
    {
        var request = new BuildingRequest.GetById
        {
            BuildingId = 1,
            CampusId = 1
        };

        var (rsp, result) = await app.Client.POSTAsync<GetBuildingById, BuildingRequest.GetById, Result<BuildingResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Get_BuildingById1WithNoCampus_Failed()
    {
        var request = new BuildingRequest.GetById
        {
            BuildingId = 1,
        };

        var (rsp, result) = await app.Client.GETAsync<GetBuildingById, BuildingRequest.GetById, Result<BuildingResponse.Detail>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.ShouldBeNull();
    }

}