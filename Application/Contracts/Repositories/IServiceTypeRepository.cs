using Domain.Entities.ServiceOrders;

namespace Application.Contracts.Repositories;

public interface IServiceTypeRepository
{
    Task<ServiceType?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceType>> GetAllAsync();
    Task AddAsync(ServiceType serviceType);
    void Update(ServiceType serviceType);
    void Remove(ServiceType serviceType);
}
