using Rise.Shared.Departments;
using Xunit.Abstractions;
using Shouldly;
using NSubstitute;
using Ardalis.Result;
using Rise.Shared.Common;

namespace Rise.Client.Departments;

/// <summary>
/// Same as <see cref="IndexShould"/> using mocking instead of faking.
/// https://nsubstitute.github.io
/// </summary>
public class IndexWithMockExampleShould : TestContext
{
    public IndexWithMockExampleShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
    }

    [Fact]
    public void ShowsDepartments()
    {
        // // Authenticate as a test user for this specific test.
        // this.AddTestAuthorization().SetAuthorized("TEST USER");
        //
        // // Mock
        // var departments = Enumerable.Range(1, 5)
        //                  .Select(i => new DepartmentDto.Index() { Id = i, Name = $"Department {i}", Description = $"Description {i}"  });
        //
        // var wrapper = new DepartmentResponse.Index
        // {
        //     Departments = departments,
        //     TotalCount = 5,
        // };
        //
        // var departmentServiceMock = Substitute.For<IDepartmentService>();
        // // Any is that we don't care about the incoming parameters. We can specify them for a specific case, but this is fine for this test.
        // departmentServiceMock.GetIndexAsync(Arg.Any<QueryRequest.SkipTake>()).Returns(Task.FromResult(Result.Success(wrapper)));
        //
        // Services.AddScoped(provider => departmentServiceMock);

        //var cut = RenderComponent<Index>();
        //cut.FindAll("table tbody tr").Count.ShouldBe(5);
    }
}
