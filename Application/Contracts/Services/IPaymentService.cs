using Application.DTOs.Payments;
using Application.Requests.Payments;

namespace Application.Contracts.Services;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<PaymentDto> CreateAsync(CreatePaymentRequest request);
    Task<bool> UpdateAsync(int id, UpdatePaymentRequest request);
    Task<bool> DeleteAsync(int id);
}
