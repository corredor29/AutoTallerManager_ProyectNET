using Application.DTOs.PurchaseOrderDetails;
using Application.Requests.PurchaseOrderDetails;

namespace Application.Contracts.Services;

public interface IPurchaseOrderDetailService
{
    Task<IEnumerable<PurchaseOrderDetailDto>> GetAllAsync();
    Task<PurchaseOrderDetailDto?> GetByIdAsync(int id);
    Task<PurchaseOrderDetailDto> CreateAsync(CreatePurchaseOrderDetailRequest request);
    Task<bool> UpdateAsync(int id, UpdatePurchaseOrderDetailRequest request);
    Task<bool> DeleteAsync(int id);
}
