using Blazor.WebAssembly.DynamicCulture.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using Rise.Client.Pages.Deadlines;
using Rise.Client.Pages.Timetable.Content;
using Rise.Shared.Deadlines;
using Rise.Shared.Lessons;
using Shouldly;
using Xunit.Abstractions;

namespace Rise.Client.Pages.Timetable;

public class TimetableIndexShould : TestContext
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TimetableIndexShould(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Services.AddXunitLogger(testOutputHelper);
        
        Services.AddScoped<ILessonService, FakeLessonService>();
        Services.AddScoped<IDeadlineService, FakeDeadlineService>();
        Services.AddMudServices();
        
        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Pages.Timetable.Timetable>>(
            new MockStringLocalizer<Rise.Client.Resources.Pages.Timetable.Timetable>());
        Services.AddSingleton<IStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>>(
            new MockStringLocalizer<Rise.Client.Resources.Layout.Loading.Loading>());
        
        var fakeLangProvider = Substitute.For<ILocalizationService>();
        Services.AddScoped<ILocalizationService>(_ => fakeLangProvider);
        
        JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        JSInterop.SetupVoid("mudResizeListenerFactory.listenForResize", _ => true);
        
        this.AddTestAuthorization().SetAuthorized("TEST USER");
    }
    
    [Fact]
    public void RendersMudhiddenStub_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        
        var cut = RenderComponent<TimetablePage>();

        cut.FindComponent<TimetableMobile>();
        cut.FindComponents<TimetableDesktop>().ShouldBeEmpty();
    }
    
    [Fact]
    public void RendersMudhiddenStub_Desktop()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.MdAndDown;
        
        var cut = RenderComponent<TimetablePage>();

        cut.FindComponent<TimetableDesktop>();
        cut.FindComponents<TimetableMobile>().ShouldBeEmpty();
    }
    
    [Fact]
    public void ShowsLessonsAndDeadlines_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        var cut = RenderComponent<TimetablePage>();

        cut.Find("[data-testid='day-Di']").Click();
        
        cut.Find("h2").TextContent.ShouldBe("MyTimetable");
        cut.FindAll(".class-schedule-block").Count.ShouldBe(1);
        cut.FindAll(".alert-section").Count.ShouldBe(1);
    }
    
    [Fact]
    public void ShowsLessonsAndDeadlines_Desktop()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.MdAndDown;
        var cut = RenderComponent<TimetablePage>();
        
        var lessons = cut.FindAll(".lesson-container");
        var lessonsByDay = lessons
            .GroupBy(l => l.GetAttribute("data-day"))
            .ToDictionary(g => g.Key, g => g.ToList());
        var mondayLessons = lessonsByDay["Monday"].Select(l => l.TextContent).ToList();
        
        cut.Find("h2").TextContent.ShouldBe("MyTimetable");
        cut.FindAll(".alert-section").Count.ShouldBe(4);
        
        lessons.Count.ShouldBe(3);
        lessonsByDay["Monday"].Count.ShouldBe(2);
        mondayLessons.ShouldSatisfyAllConditions(
            l => l.Any(text => text.Contains("RISE")).ShouldBeTrue(),
            l => l.Any(text => text.Contains("Modern Data Architectures")).ShouldBeTrue()
        );
        lessonsByDay["Tuesday"].Count.ShouldBe(1);
        lessonsByDay["Tuesday"].First().TextContent.ShouldContain("ITPCO");
    }
    
    [Fact]
    public void OpensCourseDetailsOnClick_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        var cut = RenderComponent<TimetablePage>();

        cut.Find("[data-testid='day-Di']").Click();
        
        cut.Find(".class-schedule-block").Click();
        var detailsOverlay = cut.Find(".overlay-content");
        detailsOverlay.ShouldNotBeNull(); 
        detailsOverlay.QuerySelector("h2")?.TextContent.ShouldBe("ITPCO");
        
        cut.Find(".overlay-content .btn-close").Click();
        cut.FindAll(".overlay-content").Count.ShouldBe(0);
    }
    
    [Fact]
    public void ChangesWeekToNextWeek_AndShowsNoLessons_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        var cut = RenderComponent<TimetablePage>();
        
        var initialDateText = cut.Find(".current-week p:nth-of-type(2)").TextContent;
        var initialDate = DateTime.Parse(initialDateText);
        
        cut.Find("[data-testid='day-Di']").Click();
        cut.FindAll(".class-schedule-block").Count.ShouldBeGreaterThan(0);
        
        cut.Find(".arrow-right button").Click();
        cut.FindAll(".class-schedule-block").Count.ShouldBe(0);
        
        var newDateText = cut.Find(".current-week p:nth-of-type(2)").TextContent;
        var newDate = DateTime.Parse(newDateText);
        
        (newDate - initialDate).Days.ShouldBe(7);
    }
    
    [Fact]
    public void ClickingTodayButton_ResetsWeekAndShowsLessons_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        var cut = RenderComponent<TimetablePage>();
        
        var today = DateTime.Today;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysFromMonday = (dayOfWeek + 6) % 7;
        var expectedWeekStart = today.AddDays(-daysFromMonday).Date;
        
        cut.Find("[data-testid='day-Di']").Click();
        cut.Find(".arrow-right button").Click();
        cut.Find(".lessenrooster-title .common-link-button").Click();
        
        var newDateText = cut.Find(".current-week p:nth-of-type(2)").TextContent;
        var newDate = DateTime.Parse(newDateText).Date;
        
        newDate.ShouldBe(expectedWeekStart);
    }
    
    [Fact]
    public void ClickingClassroomLink_NavigatesToCorrectCampusPage_Mobile()
    {
        ComponentFactories.Add<MudHidden, MudHiddenStub>();
        MudHiddenStub.TestBreakpoint = Breakpoint.LgAndUp;
        var cut = RenderComponent<TimetablePage>();
        var nav = Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;

        cut.Find("[data-testid='day-Di']").Click();
        cut.Find(".class-schedule-block").Click();
        
        var classroomLink = cut.Find("a[href='/campus/1/building/1/classroom/3']");
        Assert.NotNull(classroomLink);
        Assert.Equal("/campus/1/building/1/classroom/3", classroomLink.GetAttribute("href"));
    }
    
    [Fact]
    public void ShowsLoadingStateWhileLessonsAreBeingFetched()
    {
        Services.AddScoped<ILessonService>(_ => new FakeLessonService(delayMs: 200));

        var cut = RenderComponent<TimetablePage>();

        cut.Markup.ShouldContain("Loader");

        cut.WaitForState(() => 
            !cut.Markup.Contains("Loader", StringComparison.OrdinalIgnoreCase));
    }
}