using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Events;
using Rise.Shared.Infrastructure;
using Rise.Shared.Resto;

namespace Rise.Services.Infrastructure;

/// <summary>
/// Service class for managing infrastructure entities like campuses, buildings, and classrooms.
/// This service handles fetching paginated indexes for these entities.
/// Follows the service layer pattern in Clean Architecture, encapsulating business logic and data access.
/// </summary>
/// <param name="dbContext">The Entity Framework Core database context for data operations.</param>
public class InfrastructureService(ApplicationDbContext dbContext) : IInfrastructureService
{
    public async Task<Result<CampusResponse.Index>> GetCampusesIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = dbContext.Campuses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, request.OrderBy))
                : query.OrderBy(c => EF.Property<object>(c, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(c => c.Name);  // Default order
        }

        var campuses = await query
            .AsNoTracking()
            .Include(c => c.Buildings)
            .ThenInclude(b => b.Restos)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(ctx);

        return Result.Success(new CampusResponse.Index
        {
            Campuses = campuses.Select(c => new CampusDto.Index
            {
                Id = c.Id,
                Name = c.Name,
                Address = new AddressDto.Index
                {
                    PostalCode = c.Address.PostalCode,
                    City = c.Address.City,
                    AddressLine1 = c.Address.Addressline1,
                    AddressLine2 = c.Address.Addressline2,
                    
                },
                HasResto = c.Buildings.Any(b => b.Restos != null && b.Restos.Count != 0),
            }),
            TotalCount = totalCount
        });
    }

    public async Task<Result<BuildingResponse.Index>> GetBuildingsIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = dbContext.Buildings
            .Include(b => b.Campus)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(b => b.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(b => EF.Property<object>(b, request.OrderBy))
                : query.OrderBy(b => EF.Property<object>(b, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(b => b.Name);  // Default order
        }

        var buildings = await query
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(ctx);

        return Result.Success(new BuildingResponse.Index
        {
            Buildings = buildings.Select(b => new BuildingDto.Index
            {
                Id = b.Id,
                Name = b.Name,
                Address = new AddressDto.Index
                {
                    PostalCode = b.Address.PostalCode,
                    City = b.Address.City,
                    AddressLine1 = b.Address.Addressline1,
                    AddressLine2 = b.Address.Addressline2
                },
                Campus = b.Campus.Name,
                CampusId = b.Campus.Id,
            }),
            TotalCount = totalCount
        });
    }

   public async Task<Result<ClassroomResponse.Index>> GetClassroomsIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = dbContext.Classrooms
            .Include(cl => cl.Building)
            .ThenInclude(b => b.Campus)  
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(cl => cl.Description.Contains(request.SearchTerm) || cl.Category.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(ctx);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(cl => EF.Property<object>(cl, request.OrderBy))
                : query.OrderBy(cl => EF.Property<object>(cl, request.OrderBy));
        }
        else
        {
            query = query.OrderBy(cl => cl.Name);  // Default order
        }

        var classrooms = await query
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(ctx);

        return Result.Success(new ClassroomResponse.Index
        {
            Classrooms = classrooms.Select(cl => new ClassroomDto.Index
            {
                Id = cl.Id,
                Number = cl.Number,
                Name = cl.Name,
                Description = cl.Description,
                Category = cl.Category,
                Floor = cl.Floor,
                Building = new BuildingDto.Index
                {
                    Id = cl.Building.Id,
                    Name = cl.Building.Name,
                    Address = new AddressDto.Index
                    {
                        PostalCode = cl.Building.Address.PostalCode,
                        City = cl.Building.Address.City,
                        AddressLine1 = cl.Building.Address.Addressline1,
                        AddressLine2 = cl.Building.Address.Addressline2
                    },
                    Campus = cl.Building.Campus.Name,
                    CampusId = cl.Building.Campus.Id
                }
            }),
            TotalCount = totalCount
        });
    }

    public async Task<Result<InfrastructureResponse.Index>> GetInfrastructureIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var campusesTask = GetCampusesIndexAsync(request, ctx);
        var buildingsTask = GetBuildingsIndexAsync(request, ctx);
        var classroomsTask = GetClassroomsIndexAsync(request, ctx);

        await Task.WhenAll(campusesTask, buildingsTask, classroomsTask);

        var campusesResult = await campusesTask;
        var buildingsResult = await buildingsTask;
        var classroomsResult = await classroomsTask;

        if (!campusesResult.IsSuccess || !buildingsResult.IsSuccess || !classroomsResult.IsSuccess)
        {
            // Aggregate errors or return a general failure
            return Result.Conflict("Failed to fetch one or more infrastructure indexes.");
        }

        return Result.Success(new InfrastructureResponse.Index
        {
            Campuses = campusesResult.Value,
            Buildings = buildingsResult.Value,
            Classrooms = classroomsResult.Value
        });
    }

    public async Task<Result<CampusResponse.Detail>> GetCampusById(GetByIdRequest.GetById request,
        CancellationToken ctx = default)
    {
        var campus = await dbContext.Campuses
            .Include(c => c.Address)
            .Include(c => c.Facilities)
            .Include(c => c.Buildings)
            .ThenInclude(b => b.Restos)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, ctx);
        
        if (ReferenceEquals(campus, null))
            return Result.NotFound($"Campus with ID: {request.Id} not found.");
        
        return Result.Success(new CampusResponse.Detail()
        {
            Campus = new CampusDto.Detail
            {
                Id = campus.Id,
                Name = campus.Name,
                Description = campus.Description,
                Address = new AddressDto.Index
                {
                    PostalCode = campus.Address.PostalCode,
                    City = campus.Address.City,
                    AddressLine1 = campus.Address.Addressline1,
                    AddressLine2 = campus.Address.Addressline2
                },
                ImageUrl = campus.ImageUrl,
                MapsUrl = campus.MapsUrl,
                TourUrl = campus.TourUrl,
                HasResto = campus.Buildings.Any(b => b.Restos != null && b.Restos.Count != 0),
                Facilities = new FacilityDto.Index
                {
                    BikeStorage = campus.Facilities.BikeStorage,
                    Cafeteria = campus.Facilities.Cafeteria,
                    Library = campus.Facilities.Library,
                    Lockers = campus.Facilities.Lockers,
                    ParkingLot = campus.Facilities.ParkingLot,
                    RevolteRoom = campus.Facilities.RevolteRoom,
                    Restaurant = campus.Facilities.Restaurant,
                    RitaHelpdesk = campus.Facilities.RitaHelpdesk,
                    SportsHall = campus.Facilities.SportsHall,
                    Stuvo = campus.Facilities.Stuvo,
                    StudentShop = campus.Facilities.StudentShop,
                }
            }
        });
    }
    
    public async Task<Result<BuildingResponse.Detail>> GetBuildingById(BuildingRequest.GetById request,
        CancellationToken ctx = default)
    {
        var building = await dbContext.Buildings
            .Include(b => b.Address)
            .Include(b => b.Facilities)
            .Include(b => b.Campus)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BuildingId && b.Campus.Id == request.CampusId, ctx);

        if (ReferenceEquals(building, null))
            return Result.NotFound($"Campus with ID: {request.CampusId} has no building with ID: {request.BuildingId} .");
        
        return Result.Success(new BuildingResponse.Detail()
        {
            Building = new BuildingDto.Detail()
            {
                Id = building.Id,
                Name = building.Name,
                Description = building.Description,
                Address = new AddressDto.Index
                {
                    PostalCode = building.Address.PostalCode,
                    City = building.Address.City,
                    AddressLine1 = building.Address.Addressline1,
                    AddressLine2 = building.Address.Addressline2
                },
                ImageUrl = building.ImageUrl,
                Campus = building.Campus.Name,
                CampusId = building.Campus.Id,
                Facilities = new FacilityDto.Index
                {
                    BikeStorage = building.Facilities.BikeStorage,
                    Cafeteria = building.Facilities.Cafeteria,
                    Library = building.Facilities.Library,
                    Lockers = building.Facilities.Lockers,
                    ParkingLot = building.Facilities.ParkingLot,
                    RevolteRoom = building.Facilities.RevolteRoom,
                    Restaurant = building.Facilities.Restaurant,
                    RitaHelpdesk = building.Facilities.RitaHelpdesk,
                    SportsHall = building.Facilities.SportsHall,
                    Stuvo = building.Facilities.Stuvo,
                    StudentShop = building.Facilities.StudentShop,
                }
            }
        });
        
    }

    public async Task<Result<ClassroomResponse.Index>> GetClassroomById(ClassroomRequest.GetById request, CancellationToken ctx = default)
    {
        var classroom = await dbContext.Classrooms
            .AsNoTracking()
            .Select(cl => new ClassroomDto.Index
            {
                Id = cl.Id,
                Number = cl.Number,
                Name = cl.Name,
                Description = cl.Description,
                Category = cl.Category,
                Floor = cl.Floor,
                Building = new BuildingDto.Index
                {
                    Id = cl.Building.Id,
                    Name = cl.Building.Name,
                    Address = new AddressDto.Index
                    {
                        PostalCode = cl.Building.Address.PostalCode,
                        City = cl.Building.Address.City,
                        AddressLine1 = cl.Building.Address.Addressline1,
                        AddressLine2 = cl.Building.Address.Addressline2
                    },
                    Campus = cl.Building.Campus.Name,
                    CampusId = cl.Building.Campus.Id
                }
            })
            .FirstOrDefaultAsync(cl => cl.Id == request.ClassroomId 
                                       && cl.Building.Id == request.BuildingId 
                                       && cl.Building.CampusId == request.CampusId, ctx);

        if (classroom == null)
        {
            return Result.NotFound($"Classroom with ID: {request.ClassroomId} not found.");
        }

        return Result.Success(new ClassroomResponse.Index
        {
            Classrooms = new List<ClassroomDto.Index> { classroom },
            TotalCount = 1
        });
    }
    public async Task<Result<RestoResponse.Index>> GetRestosFromCampus(GetByIdRequest.GetById request, CancellationToken ctx = default)
    {
        var campus = await dbContext.Campuses
            .Include(c => c.Buildings)
            .ThenInclude(b=>b.Restos)!
            .ThenInclude(r=>r.Menu)
            
            .FirstOrDefaultAsync(c => c.Id == request.Id, ctx);

        if (campus == null)
            return Result.NotFound($"Campus with ID: {request.Id} not found.");

        if (!campus.Buildings.Any(b => b.Restos!.Count >0))
            return Result.NotFound("Campus contains no restos");

        var restos = campus.Buildings.Select(b=>b.Restos).Select(b=> b!.Select(r=> new RestoDto
        {
            Menu =
             new MenuDto
             {
                 Id = r.Menu!.Id,
                 Items = (r.Menu.Items.Count == 0)
                    ? new Dictionary<string, List<MenuItemDto>>()
                    : r.Menu.Items.ToDictionary(
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
                 HasMenu = r.Menu.HasMenu,
                 DescriptionMenu = r.Menu.DescriptionMenu!
             },
         Id = r.Id,
         Name = r.Name,
         Coordinates = r.Coordinates!,
         CampusName = r.Building!.Campus.Name
        }));
        
        return Result.Success(new RestoResponse.Index
        {
            TotalCount = campus.Buildings.Select(b=> b.Restos!.Count).Sum(),
            Restos = restos.SelectMany(r=>r)
        
        });
    }
}