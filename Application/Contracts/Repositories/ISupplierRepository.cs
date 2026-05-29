using Domain.Entities.Suppliers;

namespace Application.Contracts.Repositories;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task AddAsync(Supplier supplier);
    void Update(Supplier supplier);
    void Remove(Supplier supplier);
}
