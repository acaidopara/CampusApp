using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Services.News;

/// <summary>
/// Service for news operations.
/// </summary>
/// <param name="dbContext"></param>
public class NewsService(ApplicationDbContext dbContext) : INewsService
{
    public async Task<Result<NewsResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default)
    {
        var query = dbContext.News.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Topic))
        {
            var topic = request.Topic;

            if (!topic.Equals(Topics.All.Name, StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(n => n.Subject == topic);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Title.Contains(request.SearchTerm) ||
                                     n.AuthorName.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(n => n.CreatedAt);
        }

        var news = await query.AsNoTracking()
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
            .ToListAsync(ctx);

        return Result.Success(new NewsResponse.Detail
        {
            News = news,
            TotalCount = totalCount,
        });
    }

    public async Task<Result<NewsResponse.Index>> GetCarouselAsync(CancellationToken ctx = default)
    {
        var items = await dbContext.News
            .OrderByDescending(n => EF.Property<object>(n, "Id"))
            .Take(4)
            .ToListAsync(ctx);

        return Result.Success(new NewsResponse.Index
        {
            News = items.Select(n => new NewsDto.Index
            {
                Id = n.Id,
                Title = n.Title,
                ImageUrl = n.ImageUrl
            }),
        });
    }

    public async Task<Result<NewsResponse.DetailExtended>> GetNewsById(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {

        var article = await dbContext.News.FirstOrDefaultAsync(n => n.Id == request.Id, ctx);
        if (article == null)
        {
            return Result.NotFound($"News item with ID: {request.Id} not found.");
        }
        var dto = new NewsDto.DetailExtended
        {
            Id = article.Id,
            Title = article.Title,
            ImageUrl = article.ImageUrl,
            Subject = article.Subject,
            Date = article.Date,
            Content = article.Content,
            AuthorName = article.AuthorName,
            AuthorFunction = article.AuthorFunction,
            AuthorAvatarUrl = article.AuthorAvatarUrl,
        };

        return Result.Success(new NewsResponse.DetailExtended
        {
            News = dto
        });

    }
}
