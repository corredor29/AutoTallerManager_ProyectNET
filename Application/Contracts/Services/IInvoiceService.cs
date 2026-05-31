using Application.Common.Pagination;
using Application.DTOs.Invoices;
using Application.Filters;
using Application.Requests.Invoices;

namespace Application.Contracts.Services;

public interface IInvoiceService
{
    Task<PagedResult<InvoiceDto>> GetAllPagedAsync(PaginationParams pagination, InvoiceFilter filter);
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request);
    Task<bool> UpdateAsync(int id, UpdateInvoiceRequest request);
    Task<bool> DeleteAsync(int id);
}
