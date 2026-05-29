using Application.DTOs.PurchaseOrders;
using Application.Requests.PurchaseOrders;

namespace Application.Contracts.Services;

public interface IPurchaseOrderService
{
    Task<IEnumerable<PurchaseOrderDto>> GetAllAsync();
    Task<PurchaseOrderDto?> GetByIdAsync(int id);
    Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request);
    Task<bool> UpdateAsync(int id, UpdatePurchaseOrderRequest request);
    Task<bool> ChangeStatusAsync(int id, ChangePurchaseOrderStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
