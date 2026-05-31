using Application.Common.Pagination;
using Application.Filters;
using Domain.Entities.ServiceOrders;

namespace Application.Contracts.Repositories;

public interface IServiceOrderRepository
{
    Task<PagedResult<ServiceOrder>> GetAllPagedAsync(PaginationParams pagination, ServiceOrderFilter filter);
    Task<ServiceOrder?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceOrder>> GetAllAsync();
    Task<bool> HasOpenOrderForVehicleAsync(int vehicleId, int? excludeId = null);
    Task<bool> HasActiveOrderForVehicleAsync(int vehicleId);
    Task AddAsync(ServiceOrder serviceOrder);
    void Update(ServiceOrder serviceOrder);
    void Remove(ServiceOrder serviceOrder);
}
