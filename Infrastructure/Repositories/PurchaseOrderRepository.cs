using Application.Contracts.Repositories;
using Domain.Entities.Suppliers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        return await _dbContext.PurchaseOrders
            .Include(x => x.Supplier)
            .Include(x => x.User)
                .ThenInclude(x => x.Person)
            .Include(x => x.Status)
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
    {
        return await _dbContext.PurchaseOrders
            .Include(x => x.Supplier)
            .Include(x => x.User)
                .ThenInclude(x => x.Person)
            .Include(x => x.Status)
            .OrderByDescending(x => x.OrderedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        await _dbContext.PurchaseOrders.AddAsync(purchaseOrder);
    }

    public void Update(PurchaseOrder purchaseOrder)
    {
        _dbContext.PurchaseOrders.Update(purchaseOrder);
    }

    public void Remove(PurchaseOrder purchaseOrder)
    {
        _dbContext.PurchaseOrders.Remove(purchaseOrder);
    }
}
