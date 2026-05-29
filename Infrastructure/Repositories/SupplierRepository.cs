using Application.Contracts.Repositories;
using Domain.Entities.Suppliers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public SupplierRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _dbContext.Suppliers
            .OrderBy(x => x.CompanyName.Value)
            .ToListAsync();
    }

    public async Task AddAsync(Supplier supplier)
    {
        await _dbContext.Suppliers.AddAsync(supplier);
    }

    public void Update(Supplier supplier)
    {
        _dbContext.Suppliers.Update(supplier);
    }

    public void Remove(Supplier supplier)
    {
        _dbContext.Suppliers.Remove(supplier);
    }
}
