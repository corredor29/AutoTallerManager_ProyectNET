using Application.Contracts.Repositories;
using Domain.Entities.Parts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class MeasurementUnitRepository : IMeasurementUnitRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public MeasurementUnitRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MeasurementUnit?> GetByIdAsync(int id)
    {
        return await _dbContext.MeasurementUnits.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<MeasurementUnit>> GetAllAsync()
    {
        return await _dbContext.MeasurementUnits
            .OrderBy(x => x.Name.Value)
            .ToListAsync();
    }

    public async Task AddAsync(MeasurementUnit measurementUnit)
    {
        await _dbContext.MeasurementUnits.AddAsync(measurementUnit);
    }

    public void Update(MeasurementUnit measurementUnit)
    {
        _dbContext.MeasurementUnits.Update(measurementUnit);
    }

    public void Remove(MeasurementUnit measurementUnit)
    {
        _dbContext.MeasurementUnits.Remove(measurementUnit);
    }
}
