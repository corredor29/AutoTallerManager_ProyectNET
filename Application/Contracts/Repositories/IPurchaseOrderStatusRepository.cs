using Domain.Entities.Suppliers;

namespace Application.Contracts.Repositories;

public interface IPurchaseOrderStatusRepository
{
    Task<PurchaseOrderStatus?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseOrderStatus>> GetAllAsync();
    Task AddAsync(PurchaseOrderStatus purchaseOrderStatus);
    void Update(PurchaseOrderStatus purchaseOrderStatus);
    void Remove(PurchaseOrderStatus purchaseOrderStatus);
}
