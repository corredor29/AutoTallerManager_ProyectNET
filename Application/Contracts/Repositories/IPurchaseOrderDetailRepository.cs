using Domain.Entities.Suppliers;

namespace Application.Contracts.Repositories;

public interface IPurchaseOrderDetailRepository
{
    Task<PurchaseOrderDetail?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseOrderDetail>> GetAllAsync();
    Task AddAsync(PurchaseOrderDetail purchaseOrderDetail);
    void Update(PurchaseOrderDetail purchaseOrderDetail);
    void Remove(PurchaseOrderDetail purchaseOrderDetail);
}
