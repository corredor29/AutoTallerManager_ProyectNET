using Application.DTOs.Quotations;
using Application.Requests.Quotations;

namespace Application.Contracts.Services;

public interface IQuotationService
{
    Task<IEnumerable<QuotationDto>> GetAllAsync();
    Task<QuotationDto?> GetByIdAsync(int id);
    Task<QuotationDto> CreateAsync(CreateQuotationRequest request);
    Task<bool> UpdateAsync(int id, UpdateQuotationRequest request);
    Task<bool> ChangeStatusAsync(int id, ChangeQuotationStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
