using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.News;

namespace Rise.Services.Events;

public class EventService(ApplicationDbContext dbContext) : IEventService
{
    public async Task<Result<EventResponse.Detail>> GetIndexAsync(TopicRequest.GetBasedOnTopic request, CancellationToken ctx = default)
    {
        var query = dbContext.Events
            .Where(e => e.Date.Date >= DateOnly.FromDateTime(DateTime.Today))
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(request.Topic))
        {
            var topic = request.Topic;

            if (!topic.Equals(Topics.All.Name, StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(e => e.Subject == topic);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e => e.Title.Contains(request.SearchTerm));
        }
        
        var totalCount = await query.CountAsync(ctx);
        
        var events = await query.AsNoTracking()
            .OrderBy(n => n.Date.Date)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(e => new EventDto.Detail
            {
                Id = e.Id,
                Title = e.Title,
                Date = new EventTimeSlotDto.Index
                {
                    Date = e.Date.Date,
                    StartTime = e.Date.StartTime,
                    EndTime = e.Date.EndTime
                },
                ImageUrl = e.ImageUrl,
                Subject = e.Subject,
            })
            .ToListAsync(ctx);

        return Result.Success(new EventResponse.Detail
        {
            Events = events,
            TotalCount = totalCount,
        });
    }

    public async Task<Result<EventResponse.Index>> GetCarouselAsync(CancellationToken ctx = default)
    {
        var items = await dbContext.Events
            .Where(e => e.Date.Date >= DateOnly.FromDateTime(DateTime.Today))
            .OrderBy(n => n.Date.Date)
            .Take(4)
            .ToListAsync(ctx);

        return Result.Success(new EventResponse.Index
        {
            Events = items.Select(e => new EventDto.Index
            {
                Id = e.Id,
                Title = e.Title,
                Date = new EventTimeSlotDto.Index
                {
                    Date = e.Date.Date,
                    StartTime = e.Date.StartTime,
                    EndTime = e.Date.EndTime
                },
                ImageUrl = e.ImageUrl
            }),
        });
    }

    public async Task<Result<EventResponse.DetailExtended>> GetEventById(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        var article = await dbContext.Events
            .Include(@event => @event.Date)
            .Include(@event => @event.Address)
            .FirstOrDefaultAsync(n => n.Id == request.Id, ctx);

         if (article == null)
        {
            return Result.NotFound($"Event item with ID: {request.Id} not found.");
        }
    
        var dto = new EventDto.DetailExtended()
        {
            Id = article.Id,
            Title = article.Title,
            Date = new EventTimeSlotDto.Index
            {
                Date = article.Date.Date,
                StartTime = article.Date.StartTime,
                EndTime = article.Date.EndTime
            },
            ImageUrl = article.ImageUrl,
            Subject = article.Subject,
            Content = article.Content,
            Address = new AddressDto.Index
            {
                PostalCode = article.Address.PostalCode,
                City = article.Address.City,
                AddressLine1 = article.Address.Addressline1,
                AddressLine2 = article.Address.Addressline2
            },
            Price = article.Price,
            RegisterLink = article.RegisterLink
        };
        
        return Result.Success(new EventResponse.DetailExtended
        {
            Event = dto
        });
    }
}