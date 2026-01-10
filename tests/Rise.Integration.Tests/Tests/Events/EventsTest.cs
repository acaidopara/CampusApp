using Ardalis.Result;
using FastEndpoints;
using MudBlazor.Extensions;
using Rise.Server.Endpoints.Infrastructure;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Infrastructure;
using Rise.Integration.Tests;
using EventsApi = Rise.Server.Endpoints.Events;
using Rise.Shared.Departments;
using Rise.Shared.Events;
namespace Tests.Events;

[Collection<TestCollection>]
public class EventsTest(App app) : TestBase
{
    [Fact]
    public async Task Get_Events_Succes()
    {
        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 100
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Index, TopicRequest.GetBasedOnTopic, Result<EventResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Events.Count().Equals(5);
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().ImageUrl.ShouldBeEquivalentTo("event4.png");
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().Date.Date.Equals(new DateOnly(2026, 1, 3));

    }

    [Fact]
    public async Task Search_EventsByTopicEducation_Succes()
    {
        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 100,
            Topic = "Onderwijs"
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Index, TopicRequest.GetBasedOnTopic, Result<EventResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Events.Count().Equals(3);
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().ImageUrl.Equals("event4.png");
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().Date.Date.Equals(new DateOnly(2026, 1, 3));

    }

    [Fact]
    public async Task Search_EventsByTopicWellBeing_Succes()
    {
        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 100,
            Topic = "Welzijn"
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Index, TopicRequest.GetBasedOnTopic, Result<EventResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();

        result.Value.Events.Count().Equals(1);
    }

    [Fact]
    public async Task Search_EventsByUnknowTopic_BadRequest()
    {
        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 100,
            Topic = "random topic naam"
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Index, TopicRequest.GetBasedOnTopic, Result<EventResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Put_Events_MethodNotAllowed()
    {
        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 100,
        };

        var (rsp, result) = await app.Client.PUTAsync<EventsApi.Index, TopicRequest.GetBasedOnTopic, Result<EventResponse.Index>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Get_EventsForCarousel_Succes()
    {

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Carousel, Result<EventResponse.Index>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.Events.Count().ShouldBe(4);
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().ImageUrl.Equals("event4.png");
        result.Value.Events.Where(e => e.Title.Equals("Eéndaagse opleidingen fondsenwervingsmix")).First().Date.Date.Equals(new DateOnly(2026, 1, 3));

    }

    [Fact]
    public async Task Get_EventsById1_Succes()
    {
        var request = new GetByIdRequest.GetById
        {
            Id = 1
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Article, GetByIdRequest.GetById, Result<EventResponse.DetailExtended>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.Event.ShouldNotBeNull();
        result.Value.Event.ImageUrl.Equals("event4.png");
        result.Value.Event.Date.Date.Equals(new DateOnly(2026, 1, 3));
        result.Value.Event.Address.AddressLine1.Equals("Stadsparklaan 1");
        result.Value.Event.Address.City.Equals("Gent");
        result.Value.Event.RegisterLink?.Equals("https://www.google.com/");
        result.Value.Event.Price.Equals(4.5);

    }

    [Fact]
    public async Task Get_EventsById100_NotFound()
    {
        var request = new GetByIdRequest.GetById
        {
            Id = 100
        };

        var (rsp, result) = await app.Client.GETAsync<EventsApi.Article, GetByIdRequest.GetById, Result<EventResponse.DetailExtended>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound); // wordt notfound na merge 404 branch
        result.IsSuccess.ShouldBeFalse();

    }

}