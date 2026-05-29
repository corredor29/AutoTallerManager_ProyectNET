using Domain.Entities.Invoices;

namespace Application.Contracts.Repositories;

public interface IInvoiceDetailRepository
{
    Task<InvoiceDetail?> GetByIdAsync(int id);
    Task<IEnumerable<InvoiceDetail>> GetAllAsync();
    Task AddAsync(InvoiceDetail invoiceDetail);
    void Update(InvoiceDetail invoiceDetail);
    void Remove(InvoiceDetail invoiceDetail);
}
