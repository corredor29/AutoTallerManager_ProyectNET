using Application.DTOs.Invoices;
using Application.Requests.Invoices;

namespace Application.Contracts.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request);
    Task<bool> UpdateAsync(int id, UpdateInvoiceRequest request);
    Task<bool> DeleteAsync(int id);
}
