using Application.DTOs.PaymentMethods;
using Application.Requests.PaymentMethods;

namespace Application.Contracts.Services;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
    Task<PaymentMethodDto?> GetByIdAsync(int id);
    Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodRequest request);
    Task<bool> UpdateAsync(int id, UpdatePaymentMethodRequest request);
    Task<bool> DeleteAsync(int id);
}
