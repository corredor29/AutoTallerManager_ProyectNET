using Domain.Entities.Quotations;

namespace Application.Contracts.Repositories;

public interface IQuotationDetailRepository
{
    Task<QuotationDetail?> GetByIdAsync(int id);
    Task<IEnumerable<QuotationDetail>> GetAllAsync();
    Task AddAsync(QuotationDetail quotationDetail);
    void Update(QuotationDetail quotationDetail);
    void Remove(QuotationDetail quotationDetail);
}
