using Application.DTOs.QuotationDetails;
using Application.Requests.QuotationDetails;

namespace Application.Contracts.Services;

public interface IQuotationDetailService
{
    Task<IEnumerable<QuotationDetailDto>> GetAllAsync();
    Task<QuotationDetailDto?> GetByIdAsync(int id);
    Task<QuotationDetailDto> CreateAsync(CreateQuotationDetailRequest request);
    Task<bool> UpdateAsync(int id, UpdateQuotationDetailRequest request);
    Task<bool> DeleteAsync(int id);
}
