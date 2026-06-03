using Application.Contracts.Repositories;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PartCategoryRepository : IPartCategoryRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PartCategoryRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PartCategory?> GetByIdAsync(int id)
    {
        return await _dbContext.PartCategories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PartCategory>> GetAllAsync()
    {
        var all = await _dbContext.PartCategories.ToListAsync();
        return all.OrderBy(x => x.Name.Value).ToList();
    }

    public async Task AddAsync(PartCategory partCategory)
    {
        await _dbContext.PartCategories.AddAsync(partCategory);
    }

    public void Update(PartCategory partCategory)
    {
        _dbContext.PartCategories.Update(partCategory);
    }

    public void Remove(PartCategory partCategory)
    {
        _dbContext.PartCategories.Remove(partCategory);
    }
}