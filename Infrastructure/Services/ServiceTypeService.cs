using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.ServiceTypes;
using Application.Requests.ServiceTypes;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.ServiceOrders.ServiceType;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class ServiceTypeService : IServiceTypeService
{
    private readonly IServiceTypeRepository _serviceTypeRepository;
    private readonly AutoTallerDbContext _dbContext;

    public ServiceTypeService(IServiceTypeRepository serviceTypeRepository, AutoTallerDbContext dbContext)
    {
        _serviceTypeRepository = serviceTypeRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ServiceTypeDto>> GetAllAsync()
    {
        var serviceTypes = await _serviceTypeRepository.GetAllAsync();
        return serviceTypes.Select(MapToDto);
    }

    public async Task<ServiceTypeDto?> GetByIdAsync(int id)
    {
        var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
        return serviceType is null ? null : MapToDto(serviceType);
    }

    public async Task<ServiceTypeDto> CreateAsync(CreateServiceTypeRequest request)
    {
        await EnsureNameIsUniqueAsync(request.Name);

        var serviceType = new ServiceType(
            new ServiceTypeName(request.Name),
            new ServiceDuration(request.EstimatedDurationHours));

        await _serviceTypeRepository.AddAsync(serviceType);
        await _dbContext.SaveChangesAsync();

        return MapToDto(serviceType);
    }

    public async Task<bool> UpdateAsync(int id, UpdateServiceTypeRequest request)
    {
        var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
        if (serviceType is null)
        {
            return false;
        }

        await EnsureNameIsUniqueAsync(request.Name, id);

        serviceType.Update(
            new ServiceTypeName(request.Name),
            new ServiceDuration(request.EstimatedDurationHours));

        _serviceTypeRepository.Update(serviceType);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
        if (serviceType is null)
        {
            return false;
        }

        if (await _dbContext.ServiceOrders.AnyAsync(x => x.ServiceTypeId == id) ||
            await _dbContext.Appointments.AnyAsync(x => x.ServiceTypeId == id))
        {
            throw new InvalidOperationException($"Service type {id} is being used and cannot be deleted.");
        }

        _serviceTypeRepository.Remove(serviceType);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await _dbContext.ServiceTypes.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Service type '{name}' already exists.");
        }
    }

    private static ServiceTypeDto MapToDto(ServiceType serviceType)
    {
        return new ServiceTypeDto
        {
            Id = serviceType.Id,
            Name = serviceType.Name.Value,
            EstimatedDurationHours = serviceType.EstimatedDuration.Value
        };
    }
}
