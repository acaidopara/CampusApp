using Ardalis.Result;
using FastEndpoints;
using MudBlazor.Extensions;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;
using DepartmentsApi = Rise.Server.Endpoints.Departments;
using Rise.Shared.Departments;
namespace Tests.Departments;

[Collection<TestCollection>]
public class DepartmentTest(App app) : TestBase
{
    [Fact]
    public async Task Get_Departments_Succes()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100
        };

        var (rsp, result) = await app.Client.GETAsync<DepartmentsApi.Index, QueryRequest.SkipTake, Result<DepartmentResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Departments.Count().ShouldBe(19);

    }

    [Fact]
    public async Task Search_DepartmentsByName_Succes()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "Sociaal Werk"
        };

        var (rsp, result) = await app.Client.GETAsync<DepartmentsApi.Index, QueryRequest.SkipTake, Result<DepartmentResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Departments.Count().ShouldBeEquivalentTo(1);
        result.Value.Departments.First().DepartmentType.ShouldBeEquivalentTo("Department");

        result.Value.Departments.First().Description.ShouldBeEquivalentTo("Programma's in sociaal werk, pedagogiek en gemeenschapsontwikkeling");
        result.Value.Departments.First().Manager?.Email.ShouldBeEquivalentTo("rudi.madalijns@hogent.be");
        result.Value.Departments.First().Manager?.LastName.ShouldBeEquivalentTo("Madalijns");
        result.Value.Departments.First().Manager?.FirstName.ShouldBeEquivalentTo("Rudi");
        result.Value.Departments.First().Name.ShouldBeEquivalentTo("Sociaal Werk");
    }

    [Fact]
    public async Task Search_DepartmentsByDescription_Succes()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "Programma's in sociaal werk, pedagogiek en gemeenschapsontwikkeling"
        };

        var (rsp, result) = await app.Client.GETAsync<DepartmentsApi.Index, QueryRequest.SkipTake, Result<DepartmentResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Departments.Count().ShouldBeEquivalentTo(1);
        result.Value.Departments.First().DepartmentType.ShouldBeEquivalentTo("Department");

        result.Value.Departments.First().Description.ShouldBeEquivalentTo("Programma's in sociaal werk, pedagogiek en gemeenschapsontwikkeling");
        result.Value.Departments.First().Manager?.Email.ShouldBeEquivalentTo("rudi.madalijns@hogent.be");
        result.Value.Departments.First().Manager?.LastName.ShouldBeEquivalentTo("Madalijns");
        result.Value.Departments.First().Manager?.FirstName.ShouldBeEquivalentTo("Rudi");
        result.Value.Departments.First().Name.ShouldBeEquivalentTo("Sociaal Werk");
    }

    [Fact]
    public async Task Search_DepartmentsByUnknwonDescription_EmptyResult()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
            SearchTerm = "onbekende department"
        };

        var (rsp, result) = await app.Client.GETAsync<DepartmentsApi.Index, QueryRequest.SkipTake, Result<DepartmentResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Departments.Count().ShouldBeEquivalentTo(0);
    }


    [Fact]
    public async Task Put_Departments_MethodNotAllowed()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
        };

        var (rsp, result) = await app.Client.PUTAsync<DepartmentsApi.Index, QueryRequest.SkipTake, Result<DepartmentResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();

    }

}