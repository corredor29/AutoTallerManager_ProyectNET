using Application.Contracts.Repositories;
using Domain.Entities.Suppliers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderDetailRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PurchaseOrderDetail?> GetByIdAsync(int id)
    {
        return await _dbContext.PurchaseOrderDetails
            .Include(x => x.Part)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetAllAsync()
    {
        return await _dbContext.PurchaseOrderDetails
            .Include(x => x.Part)
            .OrderBy(x => x.PurchaseOrderId)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseOrderDetail purchaseOrderDetail)
    {
        await _dbContext.PurchaseOrderDetails.AddAsync(purchaseOrderDetail);
    }

    public void Update(PurchaseOrderDetail purchaseOrderDetail)
    {
        _dbContext.PurchaseOrderDetails.Update(purchaseOrderDetail);
    }

    public void Remove(PurchaseOrderDetail purchaseOrderDetail)
    {
        _dbContext.PurchaseOrderDetails.Remove(purchaseOrderDetail);
    }
}
