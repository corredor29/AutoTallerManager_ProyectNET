using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PurchaseOrderDetails;
using Application.Requests.PurchaseOrderDetails;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Suppliers.PurchaseOrder;
using Domain.ValueObject.Suppliers.PurchaseOrderDetail;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PurchaseOrderDetailService : IPurchaseOrderDetailService
{
    private readonly IPurchaseOrderDetailRepository _purchaseOrderDetailRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderDetailService(IPurchaseOrderDetailRepository purchaseOrderDetailRepository, AutoTallerDbContext dbContext)
    {
        _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PurchaseOrderDetailDto>> GetAllAsync()
    {
        var details = await _purchaseOrderDetailRepository.GetAllAsync();
        return details.Select(MapToDto);
    }

    public async Task<PurchaseOrderDetailDto?> GetByIdAsync(int id)
    {
        var detail = await _purchaseOrderDetailRepository.GetByIdAsync(id);
        return detail is null ? null : MapToDto(detail);
    }

    public async Task<PurchaseOrderDetailDto> CreateAsync(CreatePurchaseOrderDetailRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.PurchaseOrderId, request.PartId);
        await EnsurePartIsNotRepeatedAsync(request.PurchaseOrderId, request.PartId);

        var detail = new PurchaseOrderDetail(
            request.PurchaseOrderId,
            request.PartId,
            new PurchaseOrderDetailQuantity(request.Quantity),
            new PurchaseOrderDetailUnitPrice(request.UnitPrice));

        await _purchaseOrderDetailRepository.AddAsync(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculatePurchaseOrderTotalAsync(request.PurchaseOrderId);

        var createdDetail = await _purchaseOrderDetailRepository.GetByIdAsync(detail.Id)
            ?? throw new InvalidOperationException("Purchase order detail could not be reloaded after creation.");

        return MapToDto(createdDetail);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePurchaseOrderDetailRequest request)
    {
        var detail = await _purchaseOrderDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        detail.Update(
            new PurchaseOrderDetailQuantity(request.Quantity),
            new PurchaseOrderDetailUnitPrice(request.UnitPrice));

        _purchaseOrderDetailRepository.Update(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculatePurchaseOrderTotalAsync(detail.PurchaseOrderId);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var detail = await _purchaseOrderDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        var purchaseOrderId = detail.PurchaseOrderId;
        _purchaseOrderDetailRepository.Remove(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculatePurchaseOrderTotalAsync(purchaseOrderId);
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int purchaseOrderId, int partId)
    {
        if (!await _dbContext.PurchaseOrders.AnyAsync(x => x.Id == purchaseOrderId))
        {
            throw new ArgumentException($"Purchase order {purchaseOrderId} does not exist.");
        }

        if (!await _dbContext.Parts.AnyAsync(x => x.Id == partId))
        {
            throw new ArgumentException($"Part {partId} does not exist.");
        }
    }

    private async Task EnsurePartIsNotRepeatedAsync(int purchaseOrderId, int partId)
    {
        var exists = await _dbContext.PurchaseOrderDetails.AnyAsync(x =>
            x.PurchaseOrderId == purchaseOrderId &&
            x.PartId == partId);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Part {partId} is already assigned to purchase order {purchaseOrderId}.");
        }
    }

    private async Task RecalculatePurchaseOrderTotalAsync(int purchaseOrderId)
    {
        var purchaseOrder = await _dbContext.PurchaseOrders.FirstOrDefaultAsync(x => x.Id == purchaseOrderId)
            ?? throw new InvalidOperationException($"Purchase order {purchaseOrderId} could not be loaded.");

        var total = await _dbContext.PurchaseOrderDetails
            .Where(x => x.PurchaseOrderId == purchaseOrderId)
            .SumAsync(x => x.Quantity.Value * x.UnitPrice.Value);

        purchaseOrder.Update(new PurchaseOrderTotal(total), purchaseOrder.Notes);
        await _dbContext.SaveChangesAsync();
    }

    private static PurchaseOrderDetailDto MapToDto(PurchaseOrderDetail detail)
    {
        return new PurchaseOrderDetailDto
        {
            Id = detail.Id,
            PurchaseOrderId = detail.PurchaseOrderId,
            PartId = detail.PartId,
            PartCode = detail.Part.Code.Value,
            PartDescription = detail.Part.Description.Value,
            Quantity = detail.Quantity.Value,
            UnitPrice = detail.UnitPrice.Value,
            LineTotal = detail.Quantity.Value * detail.UnitPrice.Value
        };
    }
}
