using Application.Contracts.Repositories;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ServiceOrderPartRepository : IServiceOrderPartRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public ServiceOrderPartRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceOrderPart?> GetByIdAsync(int id)
    {
        return await _dbContext.ServiceOrderParts
            .Include(x => x.Part)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ServiceOrderPart>> GetAllAsync()
    {
        return await _dbContext.ServiceOrderParts
            .Include(x => x.Part)
            .OrderBy(x => x.ServiceOrderId)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(ServiceOrderPart serviceOrderPart)
    {
        await _dbContext.ServiceOrderParts.AddAsync(serviceOrderPart);
    }

    public void Update(ServiceOrderPart serviceOrderPart)
    {
        _dbContext.ServiceOrderParts.Update(serviceOrderPart);
    }

    public void Remove(ServiceOrderPart serviceOrderPart)
    {
        _dbContext.ServiceOrderParts.Remove(serviceOrderPart);
    }
}
