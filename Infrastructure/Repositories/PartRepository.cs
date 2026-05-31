using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Infrastructure.Extensions;
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
        var query = _dbContext.Parts
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsQueryable();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Description),
                p => EF.Functions.ILike(p.Description.Value, "%" + filter.Description!.Trim() + "%"))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Code),
                p => EF.Functions.ILike(p.Code.Value, "%" + filter.Code!.Trim() + "%"))
            .WhereIf(filter.PartCategoryId.HasValue, p => p.PartCategoryId == filter.PartCategoryId!.Value)
            .WhereIf(filter.IsActive.HasValue,       p => p.IsActive == filter.IsActive!.Value)
            .WhereIf(filter.BelowMinStock == true,   p => p.Stock.Value < p.MinStock.Value);

        var totalCount = await query.CountAsync();
        var page = pagination.NormalizedPageNumber;
        var size = pagination.NormalizedPageSize;

        var items = await query
            .OrderBy(p => p.Code.Value)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

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
        return await _dbContext.Parts
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .OrderBy(p => p.Code.Value)
            .ToListAsync();
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
