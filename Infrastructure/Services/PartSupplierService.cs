using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PartSuppliers;
using Application.Requests.PartSuppliers;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Suppliers.PartSupplier;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PartSupplierService : IPartSupplierService
{
    private readonly IPartSupplierRepository _partSupplierRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PartSupplierService(IPartSupplierRepository partSupplierRepository, AutoTallerDbContext dbContext)
    {
        _partSupplierRepository = partSupplierRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PartSupplierDto>> GetAllAsync()
    {
        var items = await _partSupplierRepository.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<PartSupplierDto?> GetByIdAsync(int id)
    {
        var item = await _partSupplierRepository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<PartSupplierDto> CreateAsync(CreatePartSupplierRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.PartId, request.SupplierId);
        await EnsurePairIsUniqueAsync(request.PartId, request.SupplierId);

        if (request.IsPrimary)
        {
            await ClearPrimarySupplierAsync(request.PartId);
        }

        var item = new PartSupplier(
            request.PartId,
            request.SupplierId,
            new PartSupplierPrice(request.PurchasePrice),
            new PartSupplierIsPrimary(request.IsPrimary));

        await _partSupplierRepository.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var createdItem = await _partSupplierRepository.GetByIdAsync(item.Id)
            ?? throw new InvalidOperationException("Part supplier could not be reloaded after creation.");

        return MapToDto(createdItem);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePartSupplierRequest request)
    {
        var item = await _partSupplierRepository.GetByIdAsync(id);
        if (item is null)
        {
            return false;
        }

        if (request.IsPrimary)
        {
            await ClearPrimarySupplierAsync(item.PartId, id);
        }

        item.Update(
            new PartSupplierPrice(request.PurchasePrice),
            new PartSupplierIsPrimary(request.IsPrimary));

        _partSupplierRepository.Update(item);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _partSupplierRepository.GetByIdAsync(id);
        if (item is null)
        {
            return false;
        }

        _partSupplierRepository.Remove(item);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int partId, int supplierId)
    {
        if (!await _dbContext.Parts.AnyAsync(x => x.Id == partId))
        {
            throw new ArgumentException($"Part {partId} does not exist.");
        }

        if (!await _dbContext.Suppliers.AnyAsync(x => x.Id == supplierId && x.IsActive))
        {
            throw new ArgumentException($"Supplier {supplierId} does not exist or is inactive.");
        }
    }

    private async Task EnsurePairIsUniqueAsync(int partId, int supplierId)
    {
        var exists = await _dbContext.PartSuppliers.AnyAsync(x =>
            x.PartId == partId &&
            x.SupplierId == supplierId);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Supplier {supplierId} is already assigned to part {partId}.");
        }
    }

    private async Task ClearPrimarySupplierAsync(int partId, int? excludeId = null)
    {
        var primaryItems = await _dbContext.PartSuppliers
            .Where(x => x.PartId == partId && x.IsPrimary.Value && (!excludeId.HasValue || x.Id != excludeId.Value))
            .ToListAsync();

        foreach (var primaryItem in primaryItems)
        {
            primaryItem.SetAsSecondary();
        }
    }

    private static PartSupplierDto MapToDto(PartSupplier item)
    {
        return new PartSupplierDto
        {
            Id = item.Id,
            PartId = item.PartId,
            PartCode = item.Part.Code.Value,
            PartDescription = item.Part.Description.Value,
            SupplierId = item.SupplierId,
            SupplierCompanyName = item.Supplier.CompanyName.Value,
            PurchasePrice = item.PurchasePrice.Value,
            IsPrimary = item.IsPrimary.Value
        };
    }
}
