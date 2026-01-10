using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Resto;

namespace Rise.Services.Resto;

/// <summary>
/// Service for projects. Note the use of <see cref="ISessionContextProvider"/> to get the current user in this layer of the application.
/// </summary>
/// <param name="dbContext"></param>
public class RestoService(ApplicationDbContext dbContext) : IRestoService
{
    public async Task<Result<RestoResponse.Index>> GetIndexAsync(SearchRequest.SkipTake request, CancellationToken ctx)
    {
        var query = dbContext.Restos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                (p.Name == request.SearchTerm
                || (p.Building != null && p.Building.Name == request.SearchTerm)
                || (p.Building != null && p.Building.Campus.Name == request.SearchTerm))

            );
        }
        var totalCount = await query.CountAsync(ctx);

    // Apply ordering
    if (!string.IsNullOrWhiteSpace(request.OrderBy))
    {
        query = request.OrderDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
            : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
    }
    else
    {
        // Default order
        query = query.OrderBy(d => d.Id);
    }
    
    var result = query
     .AsNoTracking()
     .Skip(request.Skip)
     .Take(request.Take)
     .Include(r => r.Menu)
     .Include(r => r.Building)
     .Include(r => r.Building!.Campus)
     .AsEnumerable()
     .Select(d => new RestoDto
     {
         Menu =
             new MenuDto
             {
                 Id = d.Menu!.Id,
                 Items = d.Menu.Items.Count == 0
                    ? new Dictionary<string, List<MenuItemDto>>()
                    : d.Menu.Items.ToDictionary(
                        kvp => kvp.Key,
                        kvp => (kvp.Value)
                            .Select(item => new MenuItemDto
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Description = item.Description,
                                IsVeganAndHalal = item.IsVeganAndHalal,
                                IsVeggieAndHalal = item.IsVeggieAndHalal,
                                FoodCategory = item.Category
                            })
                            .ToList()
                    ),
                 HasMenu = d.Menu.HasMenu,
                 DescriptionMenu = d.Menu.DescriptionMenu!
             },
         Id = d.Id,
         Name = d.Name,
         Coordinates = d.Coordinates!,
         CampusName = d.Building!.Campus.Name
     })
     .ToList();

        var filteredResult = result.Select(p => new RestoDto
        {
            Name = p.Name,
            Menu = new MenuDto
            {
                Items = p.Menu.Items
              .Select(kvp => new KeyValuePair<string, List<MenuItemDto>>(
                  kvp.Key,
                  kvp.Value
                      .Where(it =>
                          (!request.IsVegan.GetValueOrDefault() || it.IsVeganAndHalal) &&
                          (!request.IsVeggie.GetValueOrDefault() || it.IsVeggieAndHalal)
                      )
                      .ToList()
              ))
              .Where(kvp => kvp.Value.Any())
              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Id = p.Menu.Id,
                HasMenu = p.Menu.HasMenu,
                DescriptionMenu = p.Menu.DescriptionMenu
            },
            Id = p.Id,
            CampusName = p.CampusName,
            Coordinates = p.Coordinates
        });

        return Result.Success(new RestoResponse.Index
        {
            Restos = filteredResult,
            TotalCount = totalCount
        });
    }


}
