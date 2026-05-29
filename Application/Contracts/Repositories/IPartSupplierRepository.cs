using Domain.Entities.Suppliers;

namespace Application.Contracts.Repositories;

public interface IPartSupplierRepository
{
    Task<PartSupplier?> GetByIdAsync(int id);
    Task<IEnumerable<PartSupplier>> GetAllAsync();
    Task AddAsync(PartSupplier partSupplier);
    void Update(PartSupplier partSupplier);
    void Remove(PartSupplier partSupplier);
}
