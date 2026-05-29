using Application.Contracts.Repositories;
using Domain.Entities.Quotations;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class QuotationStatusRepository : IQuotationStatusRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public QuotationStatusRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuotationStatus?> GetByIdAsync(int id)
    {
        return await _dbContext.QuotationStatuses.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<QuotationStatus>> GetAllAsync()
    {
        return await _dbContext.QuotationStatuses
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(QuotationStatus quotationStatus)
    {
        await _dbContext.QuotationStatuses.AddAsync(quotationStatus);
    }

    public void Update(QuotationStatus quotationStatus)
    {
        _dbContext.QuotationStatuses.Update(quotationStatus);
    }

    public void Remove(QuotationStatus quotationStatus)
    {
        _dbContext.QuotationStatuses.Remove(quotationStatus);
    }
}
