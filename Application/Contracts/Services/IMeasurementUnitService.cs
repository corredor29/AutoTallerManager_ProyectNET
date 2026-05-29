using Application.DTOs.MeasurementUnits;
using Application.Requests.MeasurementUnits;

namespace Application.Contracts.Services;

public interface IMeasurementUnitService
{
    Task<IEnumerable<MeasurementUnitDto>> GetAllAsync();
    Task<MeasurementUnitDto?> GetByIdAsync(int id);
    Task<MeasurementUnitDto> CreateAsync(CreateMeasurementUnitRequest request);
    Task<bool> UpdateAsync(int id, UpdateMeasurementUnitRequest request);
    Task<bool> DeleteAsync(int id);
}
