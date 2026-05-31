using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Invoices;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<Invoice>> GetAllPagedAsync(
        PaginationParams pagination, InvoiceFilter filter)
    {
        var query = _dbContext.Invoices.AsQueryable();

        query = query
            .WhereIf(filter.ServiceOrderId.HasValue, x => x.ServiceOrderId == filter.ServiceOrderId!.Value)
            .WhereIf(filter.DateFrom.HasValue,       x => x.IssuedAt >= filter.DateFrom!.Value)
            .WhereIf(filter.DateTo.HasValue,
                x => x.IssuedAt <= filter.DateTo!.Value.Date.AddDays(1).AddTicks(-1));

        var totalCount = await query.CountAsync();
        var page = pagination.NormalizedPageNumber;
        var size = pagination.NormalizedPageSize;

        var items = await query
            .OrderByDescending(x => x.IssuedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResult<Invoice>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = size,
            TotalCount = totalCount
        };
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _dbContext.Invoices
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _dbContext.Invoices.AddAsync(invoice);
    }

    public void Update(Invoice invoice)
    {
        _dbContext.Invoices.Update(invoice);
    }

    public void Remove(Invoice invoice)
    {
        _dbContext.Invoices.Remove(invoice);
    }
}
