using Application.Contracts.Repositories;
using Domain.Entities.Suppliers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PartSupplierRepository : IPartSupplierRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PartSupplierRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PartSupplier?> GetByIdAsync(int id)
    {
        return await _dbContext.PartSuppliers
            .Include(x => x.Part)
            .Include(x => x.Supplier)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PartSupplier>> GetAllAsync()
    {
        return await _dbContext.PartSuppliers
            .Include(x => x.Part)
            .Include(x => x.Supplier)
            .OrderBy(x => x.PartId)
            .ThenBy(x => x.SupplierId)
            .ToListAsync();
    }

    public async Task AddAsync(PartSupplier partSupplier)
    {
        await _dbContext.PartSuppliers.AddAsync(partSupplier);
    }

    public void Update(PartSupplier partSupplier)
    {
        _dbContext.PartSuppliers.Update(partSupplier);
    }

    public void Remove(PartSupplier partSupplier)
    {
        _dbContext.PartSuppliers.Remove(partSupplier);
    }
}
