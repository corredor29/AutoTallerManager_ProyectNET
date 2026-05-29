using Domain.Entities.Invoices;

namespace Application.Contracts.Repositories;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentMethod>> GetAllAsync();
    Task AddAsync(PaymentMethod paymentMethod);
    void Update(PaymentMethod paymentMethod);
    void Remove(PaymentMethod paymentMethod);
}
