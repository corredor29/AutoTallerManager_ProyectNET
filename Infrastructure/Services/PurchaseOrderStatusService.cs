using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PurchaseOrderStatuses;
using Application.Requests.PurchaseOrderStatuses;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Suppliers.PurchaseOrderStatus;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PurchaseOrderStatusService : IPurchaseOrderStatusService
{
    private readonly IPurchaseOrderStatusRepository _purchaseOrderStatusRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderStatusService(IPurchaseOrderStatusRepository purchaseOrderStatusRepository, AutoTallerDbContext dbContext)
    {
        _purchaseOrderStatusRepository = purchaseOrderStatusRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PurchaseOrderStatusDto>> GetAllAsync()
    {
        var statuses = await _purchaseOrderStatusRepository.GetAllAsync();
        return statuses.Select(MapToDto);
    }

    public async Task<PurchaseOrderStatusDto?> GetByIdAsync(int id)
    {
        var status = await _purchaseOrderStatusRepository.GetByIdAsync(id);
        return status is null ? null : MapToDto(status);
    }

    public async Task<PurchaseOrderStatusDto> CreateAsync(CreatePurchaseOrderStatusRequest request)
    {
        await EnsureNameIsUniqueAsync(request.Name);

        var status = new PurchaseOrderStatus(new PurchaseOrderStatusName(request.Name));
        await _purchaseOrderStatusRepository.AddAsync(status);
        await _dbContext.SaveChangesAsync();

        return MapToDto(status);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePurchaseOrderStatusRequest request)
    {
        var status = await _purchaseOrderStatusRepository.GetByIdAsync(id);
        if (status is null)
        {
            return false;
        }

        await EnsureNameIsUniqueAsync(request.Name, id);

        status.Update(new PurchaseOrderStatusName(request.Name));
        _purchaseOrderStatusRepository.Update(status);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var status = await _purchaseOrderStatusRepository.GetByIdAsync(id);
        if (status is null)
        {
            return false;
        }

        if (await _dbContext.PurchaseOrders.AnyAsync(x => x.PurchaseOrderStatusId == id))
        {
            throw new InvalidOperationException($"Purchase order status {id} is being used and cannot be deleted.");
        }

        _purchaseOrderStatusRepository.Remove(status);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await _dbContext.PurchaseOrderStatuses.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Purchase order status '{name}' already exists.");
        }
    }

    private static PurchaseOrderStatusDto MapToDto(PurchaseOrderStatus status)
    {
        return new PurchaseOrderStatusDto
        {
            Id = status.Id,
            Name = status.Name.Value
        };
    }
}
