using Application.Contracts.Repositories;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PartRepository : IPartRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PartRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _dbContext.Parts
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Part>> GetAllAsync()
    {
        return await _dbContext.Parts
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .OrderBy(x => x.Code.Value)
            .ToListAsync();
    }

    public async Task AddAsync(Part part)
    {
        await _dbContext.Parts.AddAsync(part);
    }

    public void Update(Part part)
    {
        _dbContext.Parts.Update(part);
    }

    public void Remove(Part part)
    {
        _dbContext.Parts.Remove(part);
    }
}
