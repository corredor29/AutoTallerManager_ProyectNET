using Domain.Entities.Quotations;

namespace Application.Contracts.Repositories;

public interface IQuotationRepository
{
    Task<Quotation?> GetByIdAsync(int id);
    Task<IEnumerable<Quotation>> GetAllAsync();
    Task AddAsync(Quotation quotation);
    void Update(Quotation quotation);
    void Remove(Quotation quotation);
}
