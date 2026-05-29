using Application.Contracts.Repositories;
using Domain.Entities.ServiceOrders;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ServiceTypeRepository : IServiceTypeRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public ServiceTypeRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceType?> GetByIdAsync(int id)
    {
        return await _dbContext.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ServiceType>> GetAllAsync()
    {
        return await _dbContext.ServiceTypes
            .OrderBy(x => x.Name.Value)
            .ToListAsync();
    }

    public async Task AddAsync(ServiceType serviceType)
    {
        await _dbContext.ServiceTypes.AddAsync(serviceType);
    }

    public void Update(ServiceType serviceType)
    {
        _dbContext.ServiceTypes.Update(serviceType);
    }

    public void Remove(ServiceType serviceType)
    {
        _dbContext.ServiceTypes.Remove(serviceType);
    }
}
