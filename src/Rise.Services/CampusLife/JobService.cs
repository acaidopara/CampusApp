using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.CampusLife;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Services.CampusLife;

/// <summary>
/// Service for job operations.
/// </summary>
/// <param name="dbContext"></param>
public class JobService(ApplicationDbContext dbContext) : IJobService
{
    public async Task<Result<JobResponse.Index>> GetIndexAsync(TopicRequest.GetBasedOnJobCategory request,
        CancellationToken ctx)
    {
        var query = dbContext.Jobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.JobCategory))
        {
            var topic = request.JobCategory;

            if (!topic.Equals(CategoriesJob.All.Name, StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(n => n.JobCategory == topic);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.CompanyName.Contains(request.SearchTerm) ||
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
            query = query.OrderBy(s => s.Name);
        }

        var jobs = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(j => new JobDto.Index
            {
                Id = j.Id,
                Name = j.Name,
                CompanyName = j.CompanyName,
                Address = new AddressDto.Index
                {
                    PostalCode = j.Address!.PostalCode,
                    City = j.Address.City,
                    AddressLine1 = j.Address.Addressline1,
                    AddressLine2 = j.Address.Addressline2
                },
                ImageUrl = j.ImageUrl,
                JobCategory = j.JobCategory,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                Salary = j.Salary,
            })
            .ToListAsync(ctx);

        return Result.Success(new JobResponse.Index
        {
            Jobs = jobs,
            TotalCount = totalCount
        });
    }

    public async Task<Result<JobResponse.Detail>> GetJobByIdAsync(GetByIdRequest.GetById request,
        CancellationToken ctx)
    {
        var job = await dbContext.Jobs.Include(j => j.Address).Include(j => j.EmailAddress).FirstOrDefaultAsync(j => j.Id == request.Id, ctx);

        if (job == null)
        {
            return Result.NotFound($"Job with ID: {request.Id} not found.");
        }

        var dto = new JobDto.Detail()
        {
            Id = job.Id,
            Name = job.Name,
            CompanyName = job.CompanyName,
            Description = job.Description,
            Address = new AddressDto.Index
            {
                PostalCode = job.Address!.PostalCode,
                City = job.Address.City,
                AddressLine1 = job.Address.Addressline1,
                AddressLine2 = job.Address.Addressline2
            },
            WebsiteUrl = job.WebsiteUrl,
            ImageUrl = job.ImageUrl,
            JobCategory = job.JobCategory,
            EmailAddress = job.EmailAddress?.Value ?? string.Empty,
            StartDate = job.StartDate,
            EndDate = job.EndDate,
            Salary = job.Salary,
        };

        return Result.Success(new JobResponse.Detail()
        {
            Job = dto
        });
    }
}