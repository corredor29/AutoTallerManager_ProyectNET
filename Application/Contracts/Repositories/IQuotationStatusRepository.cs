using Domain.Entities.Quotations;

namespace Application.Contracts.Repositories;

public interface IQuotationStatusRepository
{
    Task<QuotationStatus?> GetByIdAsync(int id);
    Task<IEnumerable<QuotationStatus>> GetAllAsync();
    Task AddAsync(QuotationStatus quotationStatus);
    void Update(QuotationStatus quotationStatus);
    void Remove(QuotationStatus quotationStatus);
}
