using Application.DTOs.PurchaseOrderStatuses;
using Application.Requests.PurchaseOrderStatuses;

namespace Application.Contracts.Services;

public interface IPurchaseOrderStatusService
{
    Task<IEnumerable<PurchaseOrderStatusDto>> GetAllAsync();
    Task<PurchaseOrderStatusDto?> GetByIdAsync(int id);
    Task<PurchaseOrderStatusDto> CreateAsync(CreatePurchaseOrderStatusRequest request);
    Task<bool> UpdateAsync(int id, UpdatePurchaseOrderStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
