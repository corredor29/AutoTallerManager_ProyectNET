using Application.DTOs.QuotationStatuses;
using Application.Requests.QuotationStatuses;

namespace Application.Contracts.Services;

public interface IQuotationStatusService
{
    Task<IEnumerable<QuotationStatusDto>> GetAllAsync();
    Task<QuotationStatusDto?> GetByIdAsync(int id);
    Task<QuotationStatusDto> CreateAsync(CreateQuotationStatusRequest request);
    Task<bool> UpdateAsync(int id, UpdateQuotationStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
