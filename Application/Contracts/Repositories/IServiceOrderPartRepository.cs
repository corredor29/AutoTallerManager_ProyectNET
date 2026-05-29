using Domain.Entities.Parts;

namespace Application.Contracts.Repositories;

public interface IServiceOrderPartRepository
{
    Task<ServiceOrderPart?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceOrderPart>> GetAllAsync();
    Task AddAsync(ServiceOrderPart serviceOrderPart);
    void Update(ServiceOrderPart serviceOrderPart);
    void Remove(ServiceOrderPart serviceOrderPart);
}
