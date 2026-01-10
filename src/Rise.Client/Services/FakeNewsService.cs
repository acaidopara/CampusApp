using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Client.Services;

public class FakeNewsService : INewsService
{
    private readonly List<NewsDto.DetailExtended> _news =
    [
        new()
        {
            Id = 1,
            Title = "Zon, muziek en sfeer op het zomerterras!",
            Subject = Topics.Education.Name,
            Date = new DateTime(2025, 7, 15, 18,00,00),
            Content =
                "Het dorpsplein stond vol gezelligheid tijdens het jaarlijkse zomerterras. Lokale bands zorgden voor ambiance terwijl bezoekers genoten van drankjes en lekkernijen.",
            AuthorName = "Sofie Van den Broeck",
            AuthorFunction = "afdelingshoofd Zorg",
            AuthorAvatarUrl = null,
            ImageUrl = "image1.jpg"
        },

        new()
        {
            Id = 2,
            Title = "Nieuwe speeltuin opent haar poorten",
            Subject = Topics.Sports.Name,
            Date = new DateTime(2025, 8, 2, 12,00,00),
            Content =
                "Na maanden van werken is de nieuwe speeltuin eindelijk klaar. Met duurzame toestellen en een gezellige picknickzone is het de ideale plek voor jong en oud.",
            AuthorName = "Tom De Wilde",
            AuthorFunction = "afdelingshoofd Zorg",
            AuthorAvatarUrl = null,
            ImageUrl = "image1.jpg"
        },

        new()
        {
            Id = 3,
            Title = "Lokale chef verovert harten met seizoensmenu",
            Subject = Topics.WellBeing.Name,
            Date = new DateTime(2025, 9, 21),
            Content =
                "Chef Jana Peeters presenteerde een verrassend herfstmenu met lokale producten. Vooral haar pompoenrisotto en appelcrumble oogstten lof bij de gasten.",
            AuthorName = "Pieter Claes",
            AuthorFunction = "afdelingshoofd Zorg",
            AuthorAvatarUrl = null,
            ImageUrl = "image1.jpg"
        },

        new()
        {
            Id = 4,
            Title = "Fietstocht verbindt buurten met elkaar",
            Subject = Topics.Sports.Name,
            Date = new DateTime(2025, 6, 29),
            Content =
                "Meer dan 200 inwoners stapten samen op de fiets voor een tocht van 25 kilometer langs de mooiste plekjes van de gemeente.",
            AuthorName = "Leen Jacobs",
            AuthorFunction = "afdelingshoofd Zorg",
            AuthorAvatarUrl = "image1.jpg",
            ImageUrl = "image1.jpg"
        },

        new()
        {
            Id = 5,
            Title = "Herfstmarkt lokt recordaantal bezoekers",
            Subject = Topics.StudentAssociation.Name,
            Date = new DateTime(2025, 10, 6, 15,30,00),
            Content =
                "De geur van warme wafels en kruidige soep vulde de lucht tijdens de jaarlijkse herfstmarkt. Lokale handelaars en ambachtslieden toonden hun beste werk.",
            AuthorName = "Jan Vermeer",
            AuthorFunction = "afdelingshoofd Zorg",
            AuthorAvatarUrl = "image1.jpg",
            ImageUrl = "image1.jpg"
        }
    ];

    // --------------------------------------------
    // Carousel: 4 most recent articles
    // --------------------------------------------
    public Task<Result<NewsResponse.Index>> GetCarouselAsync(CancellationToken ctx = default)
    {
        var items = _news
            .OrderByDescending(n => n.Date)
            .Take(4)
            .Select(n => new NewsDto.Index
            {
                Id = n.Id,
                Title = n.Title,
                ImageUrl = n.ImageUrl,
            })
            .ToList();

        var carousel = new NewsResponse.Index
        {
            News = items
        };

        var result = Result<NewsResponse.Index>.Success(carousel);
        return Task.FromResult(result);
    }

    // --------------------------------------------
    // Filtering & pagination
    // --------------------------------------------
    public Task<Result<NewsResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default)
    {
        // Filter on topic
        var filtered = request.Topic == Topics.All.Name
            ? _news.AsEnumerable()
            : _news.Where(n => n.Subject.Equals(request.Topic, StringComparison.OrdinalIgnoreCase));

        // Filter on search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(n => n.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));

        var filteredList = filtered.ToList();
        
        var paged = filteredList
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(n => new NewsDto.Detail
            {
                Id = n.Id,
                Title = n.Title,
                ImageUrl = n.ImageUrl,
                Subject = n.Subject,
                Date = n.Date,
                Content = n.Content,
            })
            .ToList();

        var detail = new NewsResponse.Detail
        {
            News = paged,
            TotalCount = filteredList.Count
        };
        
        var result = Result<NewsResponse.Detail>.Success(detail);
        return Task.FromResult(result);
    }
    
    // --------------------------------------------
    // Get single article
    // --------------------------------------------
    public Task<Result<NewsResponse.DetailExtended>> GetNewsById(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        var newsItem = _news.FirstOrDefault(n => n.Id == request.Id);
        if (newsItem == null)
            return Task.FromResult(Result<NewsResponse.DetailExtended>.NotFound("artikel niet gevonden"));

        var response = new NewsResponse.DetailExtended
        {
            News = newsItem
        };

        var result = Result<NewsResponse.DetailExtended>.Success(response);
        return Task.FromResult(result);
    }

}