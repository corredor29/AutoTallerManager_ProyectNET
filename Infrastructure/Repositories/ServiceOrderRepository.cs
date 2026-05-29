using Application.Contracts.Repositories;
using Domain.Entities.ServiceOrders;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ServiceOrderRepository : IServiceOrderRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public ServiceOrderRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceOrder?> GetByIdAsync(int id)
    {
        return await _dbContext.ServiceOrders
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.ServiceType)
            .Include(x => x.OrderStatus)
            .Include(x => x.Mechanic)
                .ThenInclude(x => x.Person)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ServiceOrder>> GetAllAsync()
    {
        return await _dbContext.ServiceOrders
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.ServiceType)
            .Include(x => x.OrderStatus)
            .Include(x => x.Mechanic)
                .ThenInclude(x => x.Person)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasOpenOrderForVehicleAsync(int vehicleId, int? excludeId = null)
    {
        return await _dbContext.ServiceOrders.AnyAsync(x =>
            x.VehicleId == vehicleId &&
            x.ClosedAt == null &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task AddAsync(ServiceOrder serviceOrder)
    {
        await _dbContext.ServiceOrders.AddAsync(serviceOrder);
    }

    public void Update(ServiceOrder serviceOrder)
    {
        _dbContext.ServiceOrders.Update(serviceOrder);
    }

    public void Remove(ServiceOrder serviceOrder)
    {
        _dbContext.ServiceOrders.Remove(serviceOrder);
    }
}
