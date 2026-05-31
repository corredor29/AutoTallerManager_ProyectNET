using Application.Common.Pagination;
using Application.Filters;
using Domain.Entities.Invoices;

namespace Application.Contracts.Repositories;

public interface IInvoiceRepository
{
    Task<PagedResult<Invoice>> GetAllPagedAsync(PaginationParams pagination, InvoiceFilter filter);
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
    void Remove(Invoice invoice);
}
