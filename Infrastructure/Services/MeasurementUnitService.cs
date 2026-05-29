using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.MeasurementUnits;
using Application.Requests.MeasurementUnits;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.MeasurementUnit;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class MeasurementUnitService : IMeasurementUnitService
{
    private readonly IMeasurementUnitRepository _measurementUnitRepository;
    private readonly AutoTallerDbContext _dbContext;

    public MeasurementUnitService(IMeasurementUnitRepository measurementUnitRepository, AutoTallerDbContext dbContext)
    {
        _measurementUnitRepository = measurementUnitRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MeasurementUnitDto>> GetAllAsync()
    {
        var units = await _measurementUnitRepository.GetAllAsync();
        return units.Select(MapToDto);
    }

    public async Task<MeasurementUnitDto?> GetByIdAsync(int id)
    {
        var unit = await _measurementUnitRepository.GetByIdAsync(id);
        return unit is null ? null : MapToDto(unit);
    }

    public async Task<MeasurementUnitDto> CreateAsync(CreateMeasurementUnitRequest request)
    {
        await EnsureUniqueAsync(request.Name, request.Abbreviation);

        var unit = new MeasurementUnit(
            new MeasurementUnitName(request.Name),
            new MeasurementUnitAbbreviation(request.Abbreviation));

        await _measurementUnitRepository.AddAsync(unit);
        await _dbContext.SaveChangesAsync();

        return MapToDto(unit);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMeasurementUnitRequest request)
    {
        var unit = await _measurementUnitRepository.GetByIdAsync(id);
        if (unit is null)
        {
            return false;
        }

        await EnsureUniqueAsync(request.Name, request.Abbreviation, id);

        unit.Update(
            new MeasurementUnitName(request.Name),
            new MeasurementUnitAbbreviation(request.Abbreviation));

        _measurementUnitRepository.Update(unit);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var unit = await _measurementUnitRepository.GetByIdAsync(id);
        if (unit is null)
        {
            return false;
        }

        if (await _dbContext.Parts.AnyAsync(x => x.UnitId == id))
        {
            throw new InvalidOperationException($"Measurement unit {id} is being used and cannot be deleted.");
        }

        _measurementUnitRepository.Remove(unit);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureUniqueAsync(string name, string abbreviation, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var normalizedAbbreviation = abbreviation.Trim().ToLower();

        var duplicatedName = await _dbContext.MeasurementUnits.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (duplicatedName)
        {
            throw new InvalidOperationException($"Measurement unit '{name}' already exists.");
        }

        var duplicatedAbbreviation = await _dbContext.MeasurementUnits.AnyAsync(x =>
            x.Abbreviation.Value.ToLower() == normalizedAbbreviation &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (duplicatedAbbreviation)
        {
            throw new InvalidOperationException($"Measurement unit abbreviation '{abbreviation}' already exists.");
        }
    }

    private static MeasurementUnitDto MapToDto(MeasurementUnit unit)
    {
        return new MeasurementUnitDto
        {
            Id = unit.Id,
            Name = unit.Name.Value,
            Abbreviation = unit.Abbreviation.Value
        };
    }
}
