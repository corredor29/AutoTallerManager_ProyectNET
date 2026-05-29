using Domain.Entities.Parts;

namespace Application.Contracts.Repositories;

public interface IMeasurementUnitRepository
{
    Task<MeasurementUnit?> GetByIdAsync(int id);
    Task<IEnumerable<MeasurementUnit>> GetAllAsync();
    Task AddAsync(MeasurementUnit measurementUnit);
    void Update(MeasurementUnit measurementUnit);
    void Remove(MeasurementUnit measurementUnit);
}
