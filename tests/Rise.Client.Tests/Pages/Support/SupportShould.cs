using Bunit;
using Blazor.WebAssembly.DynamicCulture.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using Rise.Client.Pages.Support.Content;
using Rise.Shared.Support;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Rise.Client.Pages.Support;

public class SupportShould : TestContext
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SupportShould(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Services.AddXunitLogger(testOutputHelper);

        Services.AddScoped<ISupportService, FakeSupportService>();

        Services.AddMudServices();
        JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        JSInterop.SetupVoid("mudResizeListenerFactory.listenForResize", _ => true);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { { "ImageBasePath", "/images" } })
            .Build();
        Services.AddSingleton<IConfiguration>(config);

        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Pages.Support.Support>>(
            new MockStringLocalizer<Rise.Client.Resources.Pages.Support.Support>());
        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>>(
            new MockStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>());

        var fakeLangProvider = Substitute.For<ILocalizationService>();
        Services.AddScoped<ILocalizationService>(_ => fakeLangProvider);

        this.AddTestAuthorization().SetAuthorized("TEST USER");
    }

    [Fact]
    public void ShowsLoadingState_WhenServiceDelays()
    {
        Services.AddScoped<ISupportService>(_ => new FakeSupportService(delayMs: 200));

        var cut = RenderComponent<SupportPage>();

        cut.Markup.ShouldContain("Loader");

        cut.WaitForState(() =>
            !cut.Markup.Contains("Loader", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RendersOpeningHoursAndLogo_WhenServiceReturnsSupport()
    {
        var cut = RenderComponent<SupportPage>();

        cut.WaitForState(() => cut.FindAll(".opening-hours ul li").Count > 0);

        cut.Find("h3").TextContent.ShouldBe("OpeningHours");

        var img = cut.Find("img");
        img.GetAttribute("src").ShouldBe("/images/logo_RITA.png");

        var items = cut.FindAll(".opening-hours ul li");
        items.Count.ShouldBe(3);
        items[0].TextContent.ShouldContain("Tuesday");
        items[1].TextContent.ShouldContain("Wednesday");
        items[2].TextContent.ShouldContain("Thursday");

        var mailLink = cut.Find("a[href='mailto:rita@hogent.be']");
        mailLink.ShouldNotBeNull();
    }

    [Fact]
    public void ShowsStatusBadge_WithRitaText()
    {
        var cut = RenderComponent<SupportPage>();

        cut.WaitForState(() => cut.FindAll(".status-badge").Count > 0);

        var badge = cut.Find(".status-badge");
        badge.TextContent.ShouldStartWith("Rita is");
        (badge.TextContent.Contains("Open") || badge.TextContent.Contains("Closed")).ShouldBeTrue();
    }
}
