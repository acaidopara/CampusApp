using Ardalis.Result;
using FastEndpoints;
using MudBlazor.Extensions;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;
using IdentityApi = Rise.Server.Endpoints.Identity.Accounts;
using Rise.Shared.Departments;
using Rise.Shared.Events;
using Rise.Shared.Identity.Accounts;
namespace Tests.Identity;

[Collection<TestCollection>]
public class IdentityTest(App app) : TestBase
{
    [Fact]
    public async Task Get_CurrentUserInfoNotLoggedIn_NotFound()
    {

        var (rsp, result) = await app.Client.GETAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Get_CurrentUserInfoLoggedIn_Succes()
    {

        var (rsp, result) = await app.Student.GETAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();
        result.Value.Email.Equals(app.STUDENT_EMAIL);
        result.Value.Roles.ShouldContain("Student");
        result.Value.Claims["PreferedCampus"].ShouldBeEquivalentTo("Schoonmeersen");
        result.Value.Claims["Firstname"].ShouldBeEquivalentTo("Jane");
        result.Value.Claims["Lastname"].ShouldBeEquivalentTo("Doe");
        result.Value.Claims["Studentnumber"].ShouldBeEquivalentTo("S12345");
        result.Value.Claims["Birthdate"].ShouldBeEquivalentTo("°01.09.2007");
    }

    [Fact]
    public async Task Post_CurrentUserInfoLoggedIn_MethodNotAllowed()
    {

        var (rsp, result) = await app.Student.PUTAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Post_LoginFlow_Succes()
    {
        // niet ingelogd
        var (notLoggedInrsp, notLoggedInresult) = await app.TestClient.GETAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        notLoggedInrsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        notLoggedInresult.ShouldBeNull();
        // login
        app.TestClient = await app.Login(app.TEST_EMAIL, app.STUDENT_PWD);
        // wel ingelogd
        var (rsp, result) = await app.TestClient.GETAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();
        result.Value.Email.Equals(app.STUDENT_EMAIL);
        result.Value.Roles.ShouldContain("Student");
        result.Value.Claims["PreferedCampus"].ShouldBeEquivalentTo("Schoonmeersen");
        result.Value.Claims["Firstname"].ShouldBeEquivalentTo("test");
        result.Value.Claims["Lastname"].ShouldBeEquivalentTo("test");
        result.Value.Claims["Studentnumber"].ShouldBeEquivalentTo("S12347");
        result.Value.Claims["Birthdate"].ShouldBeEquivalentTo("°01.06.2007");
    }

    [Fact]
    public async Task Post_LoginAndLogoutFlow_Succes()
    {
        // login
        app.TestClient = await app.Login(app.TEST_EMAIL, app.STUDENT_PWD);
        //logout
        var (rspLoggedOut, resultLoggedOut) = await app.TestClient.POSTAsync<IdentityApi.Logout, Result>();

        rspLoggedOut.StatusCode.ShouldBe(HttpStatusCode.OK);
        resultLoggedOut.ShouldBeNull();

        var (notLoggedInrsp, notLoggedInresult) = await app.TestClient.GETAsync<IdentityApi.Info, Result<AccountResponse.Info>>();

        notLoggedInrsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        notLoggedInresult.ShouldBeNull();
    }

    [Fact]
    public async Task Post_LoginBadPassword_Unauthorized()
    {
        var loginRequest = new AccountRequest.Login { Email = app.TEST_EMAIL, Password = "foutief" };
        var resp = await app.TestClient.POSTAsync<IdentityApi.Login, AccountRequest.Login>(loginRequest);
        resp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Post_LoginBadEmail_Unauthorized()
    {
        var loginRequest = new AccountRequest.Login { Email = "niemand@student.hogent.be", Password = app.STUDENT_PWD };
        var resp = await app.TestClient.POSTAsync<IdentityApi.Login, AccountRequest.Login>(loginRequest);
        resp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_LoginBadEmailAndPassword_Unauthorized()
    {
        var loginRequest = new AccountRequest.Login { Email = "niemand@student.hogent.be", Password = "foutief" };
        var resp = await app.TestClient.POSTAsync<IdentityApi.Login, AccountRequest.Login>(loginRequest);
        resp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_LoginNoPassword_BadRequest()
    {
        var loginRequest = new AccountRequest.Login { Email = "niemand@student.hogent.be" };
        var resp = await app.TestClient.POSTAsync<IdentityApi.Login, AccountRequest.Login>(loginRequest);
        resp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_LoginNoEmail_BadRequest()
    {
        var loginRequest = new AccountRequest.Login { Password = "foutief" };
        var resp = await app.TestClient.POSTAsync<IdentityApi.Login, AccountRequest.Login>(loginRequest);
        resp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    protected override ValueTask TearDownAsync()
    {
        // zorgt ervoor dat de testclient uitgelogd is na elke test.
        app.TestClient = app.CreateClient();
        return ValueTask.CompletedTask;
    }

}