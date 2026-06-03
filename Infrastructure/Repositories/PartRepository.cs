using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PartRepository : IPartRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PartRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<Part>> GetAllPagedAsync(
        PaginationParams pagination, PartFilter filter)
    {
        var allParts = await _dbContext.Parts
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .ToListAsync();

        var query = allParts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Description))
            query = query.Where(p => p.Description.Value
                .Contains(filter.Description.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filter.Code))
            query = query.Where(p => p.Code.Value
                .Contains(filter.Code.Trim(), StringComparison.OrdinalIgnoreCase));

        if (filter.PartCategoryId.HasValue)
            query = query.Where(p => p.PartCategoryId == filter.PartCategoryId.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        if (filter.BelowMinStock == true)
            query = query.Where(p => p.Stock.Value < p.MinStock.Value);

        var totalCount = query.Count();
        var page       = pagination.NormalizedPageNumber;
        var size       = pagination.NormalizedPageSize;

        var items = query
            .OrderBy(p => p.Code.Value)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return new PagedResult<Part>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = size,
            TotalCount = totalCount
        };
    }

    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _dbContext.Parts
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Part>> GetAllAsync()
    {
        var all = await _dbContext.Parts
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .ToListAsync();

        return all.OrderBy(p => p.Code.Value).ToList();
    }

    public async Task AddAsync(Part part)
    {
        await _dbContext.Parts.AddAsync(part);
    }

    public void Update(Part part)
    {
        _dbContext.Parts.Update(part);
    }

    public void Remove(Part part)
    {
        _dbContext.Parts.Remove(part);
    }
}