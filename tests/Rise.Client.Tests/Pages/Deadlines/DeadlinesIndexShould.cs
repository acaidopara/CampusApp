using Blazor.WebAssembly.DynamicCulture.Services;
using Microsoft.Extensions.Localization;
using MudBlazor.Services;
using NSubstitute;
using Rise.Shared.Deadlines;
using Shouldly;
using Xunit.Abstractions;

namespace Rise.Client.Pages.Deadlines;

public class DeadlinesIndexShould : TestContext
{
    public DeadlinesIndexShould(ITestOutputHelper output)
    {
        Services.AddXunitLogger(output);
        Services.AddScoped<IDeadlineService, FakeDeadlineService>();
        Services.AddMudServices();
    
        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Pages.Deadlines.Deadline>>(
            new MockStringLocalizer<Rise.Client.Resources.Pages.Deadlines.Deadline>());
        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>>(
            new MockStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>());
        
        var fakeLangProvider = Substitute.For<ILocalizationService>();
        Services.AddScoped<ILocalizationService>(_ => fakeLangProvider);
        JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        this.AddTestAuthorization().SetAuthorized("TEST USER");
    }

    [Fact]
    public void ShowsDeadlines()
    {
        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();
        cut.Find("h2").TextContent.ShouldBe("Deadlines");
        cut.Find("input").GetAttribute("placeholder").ShouldBe("SearchPlaceholder");
        cut.Find("label").TextContent.ShouldBe("Course");
        cut.FindAll("div.event-content").Count.ShouldBe(5);
        cut.Find("label").TextContent.ShouldBe("Course");
        cut.FindAll("button").ShouldNotBeEmpty();

    }

    [Fact]
    public void FilterDeadlines()
    {
        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();
        cut.Find("input").Input("1");
        cut.FindAll("div.event-content").Count.ShouldBe(1);
    }

    [Fact]
    public void FilterOnCourse()
    {
        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();
        cut.Find("select").Change("RISE");
        cut.FindAll("div.event-content").All(d => d.TextContent.Contains("RISE")).ShouldBeTrue();
        cut.FindAll("div.event-content").Count.ShouldBe(2);
    }

    [Fact]
    public void FilterOnDueDateAscending()
    {
        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();
        cut.FindAll("div.event-content").First().TextContent.Contains("Deadline 1").ShouldBeTrue();
    }
    
    [Fact]
    public void FilterOnDueDateDescending()
    {
        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();
        cut.Find("div.sort-toggle").GetElementsByTagName("button")[0].Click();
        cut.FindAll("div.event-content").First().TextContent.Contains("Deadline 5").ShouldBeTrue();
    }
    
    [Fact]
    public void ShowsLoadingStateWhileDeadlinesAreBeingFetched()
    {
        Services.AddScoped<IDeadlineService>(_ => new FakeDeadlineService(delayMs: 200));

        var cut = RenderComponent<Rise.Client.Pages.Deadlines.Content.Index>();

        cut.Markup.ShouldContain("loader");

        cut.WaitForState(() => 
            !cut.Markup.Contains("loader", StringComparison.OrdinalIgnoreCase));

        cut.FindAll("div.event-content").Count.ShouldBe(5);
    }
}