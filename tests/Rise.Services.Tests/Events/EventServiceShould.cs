using Microsoft.EntityFrameworkCore;
using Rise.Domain.Education;
using Rise.Domain.Events;
using Rise.Persistence;
using Rise.Services.Events;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Services.Tests.Events;
[Collection("IntegrationTests")]
public class EventServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required IEventService EventService;

    public async Task InitializeAsync()
    {
         Db = await SetupDatabase.CreateDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }

    private EventService CreateService(
        ApplicationDbContext db)
    {
        return new EventService(db);
    }



    private Event CreateEvent(string title, string subject, DateOnly date)
    {
        var timeSlot = new EventTimeSlot(date, new TimeOnly(9, 0), new TimeOnly(10, 0));

        var address = new Address("Test Street 1",
            "Test Street 2",
            "TestCity",
            "1234"
        );

        return new Event
        {
            Title = title,
            Date = timeSlot,
            ImageUrl = "test.jpg",
            Subject = subject,
            Content = "Test content",
            Address = address,
            Price = 0,
            RegisterLink = "https://test.com"
        };
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnEvents()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var event2 = CreateEvent("Event2", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        Db.Events.AddRange(event1, event2);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Events.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Event1", result.Value.Events.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTerm()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var event2 = CreateEvent("Event2", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        Db.Events.AddRange(event1, event2);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            SearchTerm = "Event1",
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Events);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("Event1", result.Value.Events.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterByTopic()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var event2 = CreateEvent("Event2", "Topic2", DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        var event3 = CreateEvent("Event3", "Topic2", DateOnly.FromDateTime(DateTime.Today.AddDays(3)));
        Db.Events.AddRange(event1, event2, event3);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            Topic = "Topic2",
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Events.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.All(result.Value.Events, e => Assert.Equal("Topic2", e.Subject));
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnAllWhenTopicIsAll()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var event2 = CreateEvent("Event2", "Topic2", DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        Db.Events.AddRange(event1, event2);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            Topic = Topics.All.Name,
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Events.Count());
        Assert.Equal(2, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByDateAscending()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(10)));
        var event2 = CreateEvent("Event2", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(5)));
        var event3 = CreateEvent("Event3", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(20)));
        Db.Events.AddRange(event1, event2, event3);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);

        var events = result.Value.Events.ToList();

        Assert.Equal(3, events.Count);
        Assert.Equal("Event2", events[0].Title);
        Assert.Equal("Event1", events[1].Title);
        Assert.Equal("Event3", events[2].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnOnlyUpcomingEvents()
    {
        // bypass domain validation
        await Db.Database.ExecuteSqlInterpolatedAsync($"""
                                                        INSERT INTO Events (Title, ImageUrl, Subject, Content, Price, RegisterLink, EventDate, StartTime, EndTime, AddresLine1, AddresLine2, City, PostalCode, Address_Id)
                                                        VALUES ('Past', 'test.jpg', 'Topic1', 'Test content', 0, 'https://test.com', {DateOnly.FromDateTime(DateTime.Today.AddDays(-1))}, {new TimeOnly(9, 0)}, {new TimeOnly(10, 0)}, 'Test Street 1', 'Test Street 2', 'TestCity', '1234', 0)
                                                        """);
        
        await Db.Database.ExecuteSqlInterpolatedAsync($"""
                                                        INSERT INTO Events (Title, ImageUrl, Subject, Content, Price, RegisterLink, EventDate, StartTime, EndTime, AddresLine1, AddresLine2, City, PostalCode, Address_Id)
                                                        VALUES ('Today', 'test.jpg', 'Topic1', 'Test content', 0, 'https://test.com', {DateOnly.FromDateTime(DateTime.Today)}, {new TimeOnly(9, 0)}, {new TimeOnly(10, 0)}, 'Test Street 1', 'Test Street 2', 'TestCity', '1234', 0)
                                                        """);
        var futureEvent = CreateEvent("Future", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        Db.Events.Add(futureEvent);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await EventService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Events.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Contains(result.Value.Events, e => e.Title == "Today");
        Assert.Contains(result.Value.Events, e => e.Title == "Future");
    }

    [Fact]
    public async Task GetCarouselAsync_ShouldReturnFourUpcomingEventsOrderedByDate()
    {
        var event1 = CreateEvent("Event1", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(10)));
        var event2 = CreateEvent("Event2", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(5)));
        var event3 = CreateEvent("Event3", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(20)));
        var event4 = CreateEvent("Event4", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(15)));
        var event5 = CreateEvent("Event5", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(25)));
        Db.Events.AddRange(event1, event2, event3, event4, event5);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var result = await EventService.GetCarouselAsync();

        Assert.True(result.IsSuccess);

        var events = result.Value.Events.ToList();

        Assert.Equal(4, events.Count);
        Assert.Equal("Event2", events[0].Title);
        Assert.Equal("Event1", events[1].Title);
        Assert.Equal("Event4", events[2].Title);
        Assert.Equal("Event3", events[3].Title);
    }

    [Fact]
    public async Task GetEventById_ShouldReturnEvent()
    {
        var ev = CreateEvent("TestEvent", "Topic1", DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        Db.Events.Add(ev);

        await Db.SaveChangesAsync();

        EventService = CreateService(Db);

        var request = new GetByIdRequest.GetById
        {
            Id = ev.Id
        };

        var result = await EventService.GetEventById(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("TestEvent", result.Value.Event.Title);
        Assert.Equal("Topic1", result.Value.Event.Subject);
        Assert.Equal("test.jpg", result.Value.Event.ImageUrl);
        Assert.Equal("Test content", result.Value.Event.Content);
        Assert.Equal(0, result.Value.Event.Price);
        Assert.Equal("https://test.com", result.Value.Event.RegisterLink);
        Assert.Equal("1234", result.Value.Event.Address.PostalCode);
        Assert.Equal("TestCity", result.Value.Event.Address.City);
    }

    [Fact]
    public async Task GetEventById_ShouldThrow_WhenEventNotExists()
    {
        EventService = CreateService(Db);

        var request = new GetByIdRequest.GetById
        {
            Id = 999
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => EventService.GetEventById(request));
    }
}