using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.CampusLife;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Services.CampusLife;

/// <summary>
/// Service for student deals operations.
/// </summary>
/// <param name="dbContext"></param>
/// <param name="sessionContextProvider"></param>
public class StudentDealService(ApplicationDbContext dbContext) : IStudentDealService
{
    public async Task<Result<StudentDealResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnPromoCategory request,
        CancellationToken ctx)
    {
        var query = dbContext.StudentDeals.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.PromoCategory))
        {
            var topic = request.PromoCategory;

            if (!topic.Equals(CategoriesPromo.All.Name, StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(d => d.PromoCategory == topic);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Store.Contains(request.SearchTerm) || 
                                     n.Name.Contains(request.SearchTerm));
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
            query = query.OrderBy(s => s.DueDate);
        }

        
        var deals = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(s => new StudentDealDto.Index
            {
                Id = s.Id,
                Store = s.Store,        
                Name = s.Name,
                Discount = s.Discount,
                DueDate = s.DueDate,
                PromoCategory = s.PromoCategory,
                ImageUrl = s.ImageUrl,
            })
            .ToListAsync(ctx);

        return Result.Success(new StudentDealResponse.Index
        {
            StudentDeals = deals,
            TotalCount = totalCount
        });
    }
    
    public async Task<Result<StudentDealResponse.Detail>> GetStudentDealByIdAsync(GetByIdRequest.GetById request,
        CancellationToken ctx)
    {
        var deal = await dbContext.StudentDeals.FirstOrDefaultAsync(d => d.Id == request.Id, ctx);
        
        if (deal == null)
        {
            return Result.NotFound($"Deal with ID: {request.Id} not found.");
        }

        var dto = new StudentDealDto.Detail()
        {
            Id = deal.Id,
            Store = deal.Store,
            Name = deal.Name,
            Discount = deal.Discount,
            Description = deal.Description,
            DueDate = deal.DueDate,
            PromoCategory = deal.PromoCategory,
            WebUrl = deal.WebUrl,
            ImageUrl = deal.ImageUrl,
            DiscountCode = deal.DiscountCode
        };
        
        return Result.Success(new StudentDealResponse.Detail()
        {
            StudentDeal = dto
        });
    }
}