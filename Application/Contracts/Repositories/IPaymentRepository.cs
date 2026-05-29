using Domain.Entities.Invoices;

namespace Application.Contracts.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task AddAsync(Payment payment);
    void Update(Payment payment);
    void Remove(Payment payment);
}
