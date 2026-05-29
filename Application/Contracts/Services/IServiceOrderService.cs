using Application.DTOs.ServiceOrders;
using Application.Requests.ServiceOrders;

namespace Application.Contracts.Services;

public interface IServiceOrderService
{
    Task<IEnumerable<ServiceOrderDto>> GetAllAsync();
    Task<ServiceOrderDto?> GetByIdAsync(int id);
    Task<ServiceOrderDto> CreateAsync(CreateServiceOrderRequest request);
    Task<bool> UpdateAsync(int id, UpdateServiceOrderRequest request);
    Task<bool> ChangeStatusAsync(int id, ChangeServiceOrderStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
