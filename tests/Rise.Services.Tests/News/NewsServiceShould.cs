using Rise.Persistence;
using Rise.Services.News;
using Rise.Shared.Common;
using Rise.Shared.News;
using Rise.Shared.Events;

namespace Rise.Services.Tests.News;
[Collection("IntegrationTests")]
public class NewsServiceShould : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required INewsService NewsService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
        NewsService = new NewsService(Db);
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }
    
    private Domain.News.News CreateNews(string title, string subject, DateTime date)
    {
        return new Domain.News.News
        {
            Title = title,
            Date = date,
            ImageUrl = "test.jpg",
            Subject = subject,
            Content = "Test content",
            AuthorName = "Test Author",
            AuthorFunction = "Test Function",
            AuthorAvatarUrl = "test-avatar.jpg"
        };
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnNews()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        var news2 = CreateNews("News2", "Topic1", DateTime.Today.AddDays(2));
        Db.News.AddRange(news1, news2);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.News.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("News1", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterBySearchTerm()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        var news2 = CreateNews("News2", "Topic1", DateTime.Today.AddDays(2));
        Db.News.AddRange(news1, news2);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            SearchTerm = "News1",
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal("News1", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldFilterByTopic()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        var news2 = CreateNews("News2", "Topic2", DateTime.Today.AddDays(2));
        var news3 = CreateNews("News3", "Topic2", DateTime.Today.AddDays(3));
        Db.News.AddRange(news1, news2, news3);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            Topic = "Topic2",
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.News.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.All(result.Value.News, e => Assert.Equal("Topic2", e.Subject));
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnAllWhenTopicIsAll()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        var news2 = CreateNews("News2", "Topic2", DateTime.Today.AddDays(2));
        Db.News.AddRange(news1, news2);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            Topic = Topics.All.Name,
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.News.Count());
        Assert.Equal(2, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByCreatedAtAscendingByDefault()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        Db.News.Add(news1);
        await Db.SaveChangesAsync();
        await Task.Delay(1000); // to make CreatedAt different
        var news2 = CreateNews("News2", "Topic1", DateTime.Today.AddDays(2));
        Db.News.Add(news2);
        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);

        var newsItems = result.Value.News.ToList();

        Assert.Equal(2, newsItems.Count);
        Assert.Equal("News1", newsItems[0].Title);
        Assert.Equal("News2", newsItems[1].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByDateAscending()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(10));
        var news2 = CreateNews("News2", "Topic1", DateTime.Today.AddDays(5));
        var news3 = CreateNews("News3", "Topic1", DateTime.Today.AddDays(20));
        Db.News.AddRange(news1, news2, news3);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            OrderBy = "Date",
            OrderDescending = false,
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);

        var newsItems = result.Value.News.ToList();

        Assert.Equal(3, newsItems.Count);
        Assert.Equal("News2", newsItems[0].Title);
        Assert.Equal("News1", newsItems[1].Title);
        Assert.Equal("News3", newsItems[2].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnAllNews()
    {
        var pastNews = CreateNews("Past", "Topic1", DateTime.Today.AddDays(-1));
        var todayNews = CreateNews("Today", "Topic1", DateTime.Today);
        var futureNews = CreateNews("Future", "Topic1", DateTime.Today.AddDays(1));
        Db.News.AddRange(pastNews, todayNews, futureNews);

        await Db.SaveChangesAsync();

  

        var request = new TopicRequest.GetBasedOnTopic
        {
            Skip = 0,
            Take = 10
        };

        var result = await NewsService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.News.Count());
        Assert.Equal(3, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetCarouselAsync_ShouldReturnFourNewsOrderedByIdDescending()
    {
        var news1 = CreateNews("News1", "Topic1", DateTime.Today.AddDays(1));
        Db.News.Add(news1);
        await Db.SaveChangesAsync();
        var news2 = CreateNews("News2", "Topic1", DateTime.Today.AddDays(2));
        Db.News.Add(news2);
        await Db.SaveChangesAsync();
        var news3 = CreateNews("News3", "Topic1", DateTime.Today.AddDays(3));
        Db.News.Add(news3);
        await Db.SaveChangesAsync();
        var news4 = CreateNews("News4", "Topic1", DateTime.Today.AddDays(4));
        Db.News.Add(news4);
        await Db.SaveChangesAsync();
        var news5 = CreateNews("News5", "Topic1", DateTime.Today.AddDays(5));
        Db.News.Add(news5);
        await Db.SaveChangesAsync();

  

        var result = await NewsService.GetCarouselAsync();

        Assert.True(result.IsSuccess);

        var newsItems = result.Value.News.ToList();

        Assert.Equal(4, newsItems.Count);
        Assert.Equal("News5", newsItems[0].Title);
        Assert.Equal("News4", newsItems[1].Title);
        Assert.Equal("News3", newsItems[2].Title);
        Assert.Equal("News2", newsItems[3].Title);
    }

    [Fact]
    public async Task GetNewsById_ShouldReturnNews()
    {
        var news = CreateNews("TestNews", "Topic1", DateTime.Today.AddDays(1));
        Db.News.Add(news);

        await Db.SaveChangesAsync();

  

        var request = new GetByIdRequest.GetById
        {
            Id = news.Id
        };

        var result = await NewsService.GetNewsById(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("TestNews", result.Value.News.Title);
        Assert.Equal("Topic1", result.Value.News.Subject);
        Assert.Equal("test.jpg", result.Value.News.ImageUrl);
        Assert.Equal("Test content", result.Value.News.Content);
        Assert.Equal("Test Author", result.Value.News.AuthorName);
        Assert.Equal("Test Function", result.Value.News.AuthorFunction);
        Assert.Equal("test-avatar.jpg", result.Value.News.AuthorAvatarUrl);
    }

    [Fact]
    public async Task GetNewsById_ShouldThrow_WhenNewsNotExists()
    {
  

        var request = new GetByIdRequest.GetById
        {
            Id = 999
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => NewsService.GetNewsById(request));
    }
}