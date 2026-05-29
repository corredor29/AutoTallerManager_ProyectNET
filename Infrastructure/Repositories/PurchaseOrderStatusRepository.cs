using Application.Contracts.Repositories;
using Domain.Entities.Suppliers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PurchaseOrderStatusRepository : IPurchaseOrderStatusRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderStatusRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PurchaseOrderStatus?> GetByIdAsync(int id)
    {
        return await _dbContext.PurchaseOrderStatuses.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PurchaseOrderStatus>> GetAllAsync()
    {
        return await _dbContext.PurchaseOrderStatuses
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseOrderStatus purchaseOrderStatus)
    {
        await _dbContext.PurchaseOrderStatuses.AddAsync(purchaseOrderStatus);
    }

    public void Update(PurchaseOrderStatus purchaseOrderStatus)
    {
        _dbContext.PurchaseOrderStatuses.Update(purchaseOrderStatus);
    }

    public void Remove(PurchaseOrderStatus purchaseOrderStatus)
    {
        _dbContext.PurchaseOrderStatuses.Remove(purchaseOrderStatus);
    }
}
