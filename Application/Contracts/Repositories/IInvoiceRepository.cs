using Domain.Entities.Invoices;

namespace Application.Contracts.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
    void Remove(Invoice invoice);
}
