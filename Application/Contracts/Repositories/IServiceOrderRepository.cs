using Domain.Entities.ServiceOrders;

namespace Application.Contracts.Repositories;

public interface IServiceOrderRepository
{
    Task<ServiceOrder?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceOrder>> GetAllAsync();
    Task<bool> HasOpenOrderForVehicleAsync(int vehicleId, int? excludeId = null);
    Task AddAsync(ServiceOrder serviceOrder);
    void Update(ServiceOrder serviceOrder);
    void Remove(ServiceOrder serviceOrder);
}
