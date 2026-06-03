using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Suppliers;
using Application.Requests.Suppliers;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Suppliers.Supplier;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly AutoTallerDbContext  _dbContext;

    public SupplierService(ISupplierRepository supplierRepository, AutoTallerDbContext dbContext)
    {
        _supplierRepository = supplierRepository;
        _dbContext          = dbContext;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToDto);
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier is null ? null : MapToDto(supplier);
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request)
    {
        await EnsureTaxIdIsUniqueAsync(request.TaxId);

        var supplier = new Supplier(
            new CompanyName(request.CompanyName),
            new TaxId(request.TaxId),
            new SupplierContactName(request.ContactName),
            new SupplierPhone(request.Phone),
            new SupplierEmail(request.Email),
            new SupplierAddress(request.Address));

        await _supplierRepository.AddAsync(supplier);
        await _dbContext.SaveChangesAsync();

        return MapToDto(supplier);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSupplierRequest request)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier is null) return false;

        await EnsureTaxIdIsUniqueAsync(request.TaxId, id);

        supplier.Update(
            new CompanyName(request.CompanyName),
            new TaxId(request.TaxId),
            new SupplierContactName(request.ContactName),
            new SupplierPhone(request.Phone),
            new SupplierEmail(request.Email),
            new SupplierAddress(request.Address));

        if (request.IsActive)
            supplier.Activate();
        else
            supplier.Deactivate();

        _supplierRepository.Update(supplier);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier is null) return false;

        if (await _dbContext.PartSuppliers.AnyAsync(x => x.SupplierId == id))
            throw new InvalidOperationException($"Supplier {id} is being used and cannot be deleted.");

        _supplierRepository.Remove(supplier);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureTaxIdIsUniqueAsync(string? taxId, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(taxId)) return;

        var normalizedTaxId = taxId.Trim().ToLower();

        // ── Fix: ToListAsync + filtro en memoria ───────
        var all = await _dbContext.Suppliers.ToListAsync();
        var exists = all.Any(x =>
            x.TaxId?.Value != null &&
            x.TaxId.Value.ToLower() == normalizedTaxId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
            throw new InvalidOperationException($"Supplier tax ID '{taxId}' already exists.");
    }

    private static SupplierDto MapToDto(Supplier supplier) => new()
    {
        Id          = supplier.Id,
        CompanyName = supplier.CompanyName?.Value  ?? string.Empty,
        TaxId       = supplier.TaxId?.Value,
        ContactName = supplier.ContactName?.Value,
        Phone       = supplier.Phone?.Value,
        Email       = supplier.Email?.Value,
        Address     = supplier.Address?.Value,
        IsActive    = supplier.IsActive
    };
}