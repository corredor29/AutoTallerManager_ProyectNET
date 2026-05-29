using Application.DTOs.InvoiceDetails;
using Application.Requests.InvoiceDetails;

namespace Application.Contracts.Services;

public interface IInvoiceDetailService
{
    Task<IEnumerable<InvoiceDetailDto>> GetAllAsync();
    Task<InvoiceDetailDto?> GetByIdAsync(int id);
    Task<InvoiceDetailDto> CreateAsync(CreateInvoiceDetailRequest request);
    Task<bool> UpdateAsync(int id, UpdateInvoiceDetailRequest request);
    Task<bool> DeleteAsync(int id);
}
