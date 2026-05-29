using Application.Contracts.Repositories;
using Domain.Entities.Invoices;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class InvoiceDetailRepository : IInvoiceDetailRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceDetailRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoiceDetail?> GetByIdAsync(int id)
    {
        return await _dbContext.InvoiceDetails.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<InvoiceDetail>> GetAllAsync()
    {
        return await _dbContext.InvoiceDetails
            .OrderBy(x => x.InvoiceId)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(InvoiceDetail invoiceDetail)
    {
        await _dbContext.InvoiceDetails.AddAsync(invoiceDetail);
    }

    public void Update(InvoiceDetail invoiceDetail)
    {
        _dbContext.InvoiceDetails.Update(invoiceDetail);
    }

    public void Remove(InvoiceDetail invoiceDetail)
    {
        _dbContext.InvoiceDetails.Remove(invoiceDetail);
    }
}
