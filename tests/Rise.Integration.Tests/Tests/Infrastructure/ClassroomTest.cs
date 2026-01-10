using Ardalis.Result;
using FastEndpoints;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;

namespace Tests.Infrastructure;

[Collection<TestCollection>]
public class ClassroomTest(App app) : TestBase
{
    [Fact]
    public async Task Get_Classrooms_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100
        };

        var (rsp, result) = await app.Client.GETAsync<ClassroomsIndex, QueryRequest.SkipTake, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Classrooms.ShouldNotBeEmpty();
        result.Value.Classrooms.ToList().Count.ShouldBe(25);

        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).Count().ShouldBe(1);
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Category.ShouldBeEquivalentTo("laboratorium");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Building.Name.ShouldBeEquivalentTo("Gebouw B");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Building.Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Description.ShouldBeEquivalentTo("GSCHB.1.038 Labo fysica");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Number.ShouldBeEquivalentTo("038");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Floor.ShouldBeEquivalentTo("1");

        result.Value.Classrooms.Where(c => c.Floor.Equals("1")).Count().ShouldBe(17);
    }

    [Fact]
    public async Task Post_Classrooms_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var (rsp, result) = await app.Client.POSTAsync<ClassroomsIndex, QueryRequest.SkipTake, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();

    }

    [Fact]
    public async Task Search_Classroom038_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "GSCHB.1.038 Labo fysica"
        };

        var (rsp, result) = await app.Client.GETAsync<ClassroomsIndex, QueryRequest.SkipTake, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Classrooms.ShouldNotBeEmpty();
        result.Value.Classrooms.ToList().Count.ShouldBe(1);

        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Category.ShouldBeEquivalentTo("laboratorium");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Building.Name.ShouldBeEquivalentTo("Gebouw B");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Building.Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Description.ShouldBeEquivalentTo("GSCHB.1.038 Labo fysica");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Number.ShouldBeEquivalentTo("038");
        result.Value.Classrooms.Where(c => c.Name.Equals("01.02.110.038")).First().Floor.ShouldBeEquivalentTo("1");
    }

    [Fact]
    public async Task Search_ClassroomsByCategoryAuditorium_Success()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "auditorium"
        };

        var (rsp, result) = await app.Client.GETAsync<ClassroomsIndex, QueryRequest.SkipTake, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Classrooms.ShouldNotBeEmpty();
        result.Value.Classrooms.ToList().Count.ShouldBe(7);
    }

    [Fact]
    public async Task Search_NonExistantClassroom_Failed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Classroom Niet bestaand"
        };

        var (rsp, result) = await app.Client.GETAsync<ClassroomsIndex, QueryRequest.SkipTake, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Value.Classrooms.ToList().ShouldBeEmpty();
    }

    [Fact]
    public async Task Get_ClassroomById17_Success()
    {
        var request = new ClassroomRequest.GetById
        {
            ClassroomId = 17,
            CampusId = 1,
            BuildingId = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetClassroomById, ClassroomRequest.GetById, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();

        result.Value.Classrooms.ShouldNotBeEmpty();
        result.Value.Classrooms.ToList().Count.ShouldBe(1);

        result.Value.Classrooms.First().Category.ShouldBeEquivalentTo("laboratorium");
        result.Value.Classrooms.First().Building.Name.ShouldBeEquivalentTo("Gebouw B");
        result.Value.Classrooms.First().Building.Campus.ShouldBeEquivalentTo("Campus Schoonmeersen");
        result.Value.Classrooms.First().Description.ShouldBeEquivalentTo("GSCHB.1.038 Labo fysica");
        result.Value.Classrooms.First().Number.ShouldBeEquivalentTo("038");
        result.Value.Classrooms.First().Floor.ShouldBeEquivalentTo("1");
    }

    [Fact]
    public async Task Get_ClassroomById100_NotFound()
    {
        var request = new ClassroomRequest.GetById
        {
            ClassroomId = 100,
            CampusId = 1,
            BuildingId = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetClassroomById, ClassroomRequest.GetById, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();

        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Post_ClassroomById1_Failed()
    {
        var request = new ClassroomRequest.GetById
        {
            ClassroomId = 1,
            CampusId = 1,
            BuildingId = 1
        };

        var (rsp, result) = await app.Client.POSTAsync<GetClassroomById, ClassroomRequest.GetById, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Get_CLassroomById1WithNoCampus_Failed()
    {
        var request = new ClassroomRequest.GetById
        {
            ClassroomId = 1,
            BuildingId = 1
        };

        var (rsp, result) = await app.Client.GETAsync<GetClassroomById, ClassroomRequest.GetById, Result<ClassroomResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.ShouldBeNull();
    }

}