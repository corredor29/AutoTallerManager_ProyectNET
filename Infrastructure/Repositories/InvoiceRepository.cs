using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Invoices;
using Infrastructure.Context;
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
        // Traer todo a memoria primero
        var allInvoices = await _dbContext.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Vehicle)
                    .ThenInclude(x => x.Ownerships)
                        .ThenInclude(o => o.Customer)
                            .ThenInclude(c => c.Person)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.OrderStatus)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Appointment)
                    .ThenInclude(x => x!.Customer)
                        .ThenInclude(x => x.Person)
            .ToListAsync();

        // Filtrar en memoria
        var query = allInvoices.AsEnumerable();

        if (filter.ServiceOrderId.HasValue)
            query = query.Where(x => x.ServiceOrderId == filter.ServiceOrderId.Value);

        if (filter.CustomerId.HasValue)
        {
            var cid = filter.CustomerId.Value;
            query = query.Where(x =>
                GetCustomerId(x) == cid);
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerName))
        {
            var name = filter.CustomerName.Trim().ToLower();
            query = query.Where(x =>
                GetCustomerName(x).ToLower().Contains(name));
        }

        if (filter.DateFrom.HasValue)
            query = query.Where(x => x.IssuedAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(x => x.IssuedAt <= filter.DateTo.Value.Date.AddDays(1).AddTicks(-1));

        var filtered    = query.OrderByDescending(x => x.IssuedAt).ToList();
        var totalCount  = filtered.Count;
        var page        = pagination.NormalizedPageNumber;
        var size        = pagination.NormalizedPageSize;
        var items       = filtered.Skip((page - 1) * size).Take(size).ToList();

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
        return await _dbContext.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Vehicle)
                    .ThenInclude(x => x.Ownerships)
                        .ThenInclude(o => o.Customer)
                            .ThenInclude(c => c.Person)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.OrderStatus)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Appointment)
                    .ThenInclude(x => x!.Customer)
                        .ThenInclude(x => x.Person)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _dbContext.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Vehicle)
                    .ThenInclude(x => x.Ownerships)
                        .ThenInclude(o => o.Customer)
                            .ThenInclude(c => c.Person)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.OrderStatus)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x.Appointment)
                    .ThenInclude(x => x!.Customer)
                        .ThenInclude(x => x.Person)
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

    // ── Helpers ────────────────────────────────────
    private static string GetCustomerName(Invoice x)
    {
        if (x.ServiceOrder?.Appointment?.Customer?.Person != null)
        {
            var p = x.ServiceOrder.Appointment.Customer.Person;
            return $"{p.FirstName.Value} {p.LastName.Value}".Trim();
        }
        var owner = x.ServiceOrder?.Vehicle?.Ownerships?
            .FirstOrDefault(o => o.DateRange.EndDate == null);
        if (owner?.Customer?.Person != null)
        {
            var p = owner.Customer.Person;
            return $"{p.FirstName.Value} {p.LastName.Value}".Trim();
        }
        return string.Empty;
    }

    private static int? GetCustomerId(Invoice x)
    {
        if (x.ServiceOrder?.Appointment?.CustomerId != null)
            return x.ServiceOrder.Appointment.CustomerId;

        var owner = x.ServiceOrder?.Vehicle?.Ownerships?
            .FirstOrDefault(o => o.DateRange.EndDate == null);
        return owner?.CustomerId;
    }
}