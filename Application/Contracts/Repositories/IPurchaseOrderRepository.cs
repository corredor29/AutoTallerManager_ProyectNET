using Domain.Entities.Suppliers;

namespace Application.Contracts.Repositories;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseOrder>> GetAllAsync();
    Task AddAsync(PurchaseOrder purchaseOrder);
    void Update(PurchaseOrder purchaseOrder);
    void Remove(PurchaseOrder purchaseOrder);
}
