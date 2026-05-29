using Application.Contracts.Repositories;
using Domain.Entities.Quotations;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class QuotationDetailRepository : IQuotationDetailRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public QuotationDetailRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuotationDetail?> GetByIdAsync(int id)
    {
        return await _dbContext.QuotationDetails
            .Include(x => x.Part)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<QuotationDetail>> GetAllAsync()
    {
        return await _dbContext.QuotationDetails
            .Include(x => x.Part)
            .OrderBy(x => x.QuotationId)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(QuotationDetail quotationDetail)
    {
        await _dbContext.QuotationDetails.AddAsync(quotationDetail);
    }

    public void Update(QuotationDetail quotationDetail)
    {
        _dbContext.QuotationDetails.Update(quotationDetail);
    }

    public void Remove(QuotationDetail quotationDetail)
    {
        _dbContext.QuotationDetails.Remove(quotationDetail);
    }
}
