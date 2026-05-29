using Application.Contracts.Repositories;
using Domain.Entities.Quotations;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class QuotationRepository : IQuotationRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public QuotationRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Quotation?> GetByIdAsync(int id)
    {
        return await _dbContext.Quotations
            .Include(x => x.CreatedByUser)
                .ThenInclude(x => x.Person)
            .Include(x => x.QuotationStatus)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Quotation>> GetAllAsync()
    {
        return await _dbContext.Quotations
            .Include(x => x.CreatedByUser)
                .ThenInclude(x => x.Person)
            .Include(x => x.QuotationStatus)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Quotation quotation)
    {
        await _dbContext.Quotations.AddAsync(quotation);
    }

    public void Update(Quotation quotation)
    {
        _dbContext.Quotations.Update(quotation);
    }

    public void Remove(Quotation quotation)
    {
        _dbContext.Quotations.Remove(quotation);
    }
}
