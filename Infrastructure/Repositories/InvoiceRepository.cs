using Application.Contracts.Repositories;
using Domain.Entities.Invoices;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _dbContext.Invoices
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _dbContext.Invoices.AddAsync(invoice);
    }

    public void Update(Invoice invoice)
    {
        _dbContext.Invoices.Update(invoice);
    }

    public void Remove(Invoice invoice)
    {
        _dbContext.Invoices.Remove(invoice);
    }
}
