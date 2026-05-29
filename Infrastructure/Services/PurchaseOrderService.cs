using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PurchaseOrders;
using Application.Requests.PurchaseOrders;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Parts.Part;
using Domain.ValueObject.Suppliers.PurchaseOrder;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository, AutoTallerDbContext dbContext)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync()
    {
        var purchaseOrders = await _purchaseOrderRepository.GetAllAsync();
        return purchaseOrders.Select(MapToDto);
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);
        return purchaseOrder is null ? null : MapToDto(purchaseOrder);
    }

    public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.SupplierId, request.UserId, request.PurchaseOrderStatusId);

        var purchaseOrder = new PurchaseOrder(
            request.SupplierId,
            request.UserId,
            request.PurchaseOrderStatusId,
            new PurchaseOrderTotal(request.Total),
            new PurchaseOrderNotes(request.Notes));

        await _purchaseOrderRepository.AddAsync(purchaseOrder);
        await _dbContext.SaveChangesAsync();

        var createdPurchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrder.Id)
            ?? throw new InvalidOperationException("Purchase order could not be reloaded after creation.");

        return MapToDto(createdPurchaseOrder);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePurchaseOrderRequest request)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);
        if (purchaseOrder is null)
        {
            return false;
        }

        purchaseOrder.Update(
            new PurchaseOrderTotal(request.Total),
            new PurchaseOrderNotes(request.Notes));

        _purchaseOrderRepository.Update(purchaseOrder);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangePurchaseOrderStatusRequest request)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);
        if (purchaseOrder is null)
        {
            return false;
        }

        var statusName = await _dbContext.PurchaseOrderStatuses
            .Where(x => x.Id == request.PurchaseOrderStatusId)
            .Select(x => x.Name.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(statusName))
        {
            throw new ArgumentException($"Purchase order status {request.PurchaseOrderStatusId} does not exist.");
        }

        purchaseOrder.ChangeStatus(request.PurchaseOrderStatusId);

        if (statusName.Equals("Received", StringComparison.OrdinalIgnoreCase) &&
            purchaseOrder.ReceivedAt is null)
        {
            purchaseOrder.Receive();

            var details = await _dbContext.PurchaseOrderDetails
                .Where(x => x.PurchaseOrderId == id)
                .ToListAsync();

            foreach (var detail in details)
            {
                var part = await _dbContext.Parts.FirstOrDefaultAsync(x => x.Id == detail.PartId)
                    ?? throw new InvalidOperationException($"Part {detail.PartId} could not be loaded.");

                part.AddStock(new PartStock(detail.Quantity.Value));
            }
        }

        _purchaseOrderRepository.Update(purchaseOrder);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);
        if (purchaseOrder is null)
        {
            return false;
        }

        if (await _dbContext.PurchaseOrderDetails.AnyAsync(x => x.PurchaseOrderId == id))
        {
            throw new InvalidOperationException($"Purchase order {id} has details and cannot be deleted.");
        }

        _purchaseOrderRepository.Remove(purchaseOrder);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int supplierId, int userId, int purchaseOrderStatusId)
    {
        if (!await _dbContext.Suppliers.AnyAsync(x => x.Id == supplierId && x.IsActive))
        {
            throw new ArgumentException($"Supplier {supplierId} does not exist or is inactive.");
        }

        if (!await _dbContext.Users.AnyAsync(x => x.Id == userId && x.IsActive))
        {
            throw new ArgumentException($"User {userId} does not exist or is inactive.");
        }

        if (!await _dbContext.PurchaseOrderStatuses.AnyAsync(x => x.Id == purchaseOrderStatusId))
        {
            throw new ArgumentException($"Purchase order status {purchaseOrderStatusId} does not exist.");
        }
    }

    private static PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        var userName = purchaseOrder.User?.Person is null
            ? string.Empty
            : $"{purchaseOrder.User.Person.FirstName.Value} {purchaseOrder.User.Person.LastName.Value}".Trim();

        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            SupplierCompanyName = purchaseOrder.Supplier.CompanyName.Value,
            UserId = purchaseOrder.UserId,
            UserName = userName,
            PurchaseOrderStatusId = purchaseOrder.PurchaseOrderStatusId,
            PurchaseOrderStatusName = purchaseOrder.Status.Name.Value,
            OrderedAt = purchaseOrder.OrderedAt,
            ReceivedAt = purchaseOrder.ReceivedAt,
            Total = purchaseOrder.Total.Value,
            Notes = purchaseOrder.Notes.Value
        };
    }
}
