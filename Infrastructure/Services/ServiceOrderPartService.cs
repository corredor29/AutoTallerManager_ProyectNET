using System.Security.Claims;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.ServiceOrderParts;
using Application.Requests.ServiceOrderParts;
using Domain.Entities.Audit;
using Domain.Entities.Parts;
using Domain.ValueObject.Audit.AuditLog;
using Domain.ValueObject.Parts.Part;
using Domain.ValueObject.Parts.ServiceOrderPart;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class ServiceOrderPartService : IServiceOrderPartService
{
    private readonly IServiceOrderPartRepository _serviceOrderPartRepository;
    private readonly AutoTallerDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceOrderPartService(
        IServiceOrderPartRepository serviceOrderPartRepository,
        AutoTallerDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _serviceOrderPartRepository = serviceOrderPartRepository;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ServiceOrderPartDto>> GetAllAsync()
    {
        var items = await _serviceOrderPartRepository.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<ServiceOrderPartDto?> GetByIdAsync(int id)
    {
        var item = await _serviceOrderPartRepository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<ServiceOrderPartDto> CreateAsync(CreateServiceOrderPartRequest request)
    {
        var part = await EnsureRelatedEntitiesExistAsync(request.ServiceOrderId, request.PartId);
        await EnsurePartIsNotRepeatedAsync(request.ServiceOrderId, request.PartId);

        var quantity = new ServiceOrderPartQuantity(request.Quantity);
        if (!part.HasSufficientStock(quantity.Value))
        {
            throw new InvalidOperationException(
                $"Insufficient stock for part '{part.Code.Value}'. " +
                $"Available: {part.Stock.Value}, Requested: {quantity.Value}");
        }

        part.RemoveStock(new PartStock(quantity.Value));

        if (part.IsBelowMinStock())
        {
            await LogLowStockWarningAsync(part);
        }

        var item = new ServiceOrderPart(
            request.ServiceOrderId,
            request.PartId,
            quantity,
            new ServiceOrderPartUnitPrice(request.AppliedUnitPrice));

        await _serviceOrderPartRepository.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var createdItem = await _serviceOrderPartRepository.GetByIdAsync(item.Id)
            ?? throw new InvalidOperationException("Service order part could not be reloaded after creation.");

        return MapToDto(createdItem);
    }

    public async Task<bool> UpdateAsync(int id, UpdateServiceOrderPartRequest request)
    {
        var item = await _serviceOrderPartRepository.GetByIdAsync(id);
        if (item is null)
        {
            return false;
        }

        await EnsureServiceOrderIsOpenAsync(item.ServiceOrderId);

        var part = await _dbContext.Parts.FirstOrDefaultAsync(x => x.Id == item.PartId)
            ?? throw new InvalidOperationException($"Part {item.PartId} could not be loaded.");

        var previousQuantity = item.Quantity.Value;
        var newQuantity = request.Quantity;
        var difference = newQuantity - previousQuantity;

        if (difference > 0)
        {
            if (!part.HasSufficientStock(difference))
            {
                throw new InvalidOperationException($"Part {item.PartId} does not have enough stock.");
            }

            part.RemoveStock(new PartStock(difference));
        }
        else if (difference < 0)
        {
            part.AddStock(new PartStock(Math.Abs(difference)));
        }

        item.Update(
            new ServiceOrderPartQuantity(request.Quantity),
            new ServiceOrderPartUnitPrice(request.AppliedUnitPrice));

        _serviceOrderPartRepository.Update(item);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _serviceOrderPartRepository.GetByIdAsync(id);
        if (item is null)
        {
            return false;
        }

        await EnsureServiceOrderIsOpenAsync(item.ServiceOrderId);

        var part = await _dbContext.Parts.FirstOrDefaultAsync(x => x.Id == item.PartId)
            ?? throw new InvalidOperationException($"Part {item.PartId} could not be loaded.");

        part.AddStock(new PartStock(item.Quantity.Value));
        _serviceOrderPartRepository.Remove(item);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task LogLowStockWarningAsync(Part part)
    {
        var updateActionTypeId = await _dbContext.AuditActionTypes
            .Where(a => a.Name.Value.ToLower() == "update")
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        if (updateActionTypeId == 0) return;

        int? userId = null;
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated == true)
        {
            var sub = principal.FindFirstValue("sub")
                   ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(sub, out var parsed))
                userId = parsed;
        }

        userId ??= await _dbContext.Users
            .OrderBy(u => u.Id)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null) return;

        var description =
            $"Low stock warning: Part '{part.Code.Value}' has {part.Stock.Value} unit(s), " +
            $"below minimum ({part.MinStock.Value}). Reorder required.";

        _dbContext.AuditLogs.Add(new AuditLog(
            userId.Value,
            updateActionTypeId,
            new AffectedEntityName("Part"),
            part.Id,
            new AuditDescription(description)));
    }

    private async Task<Part> EnsureRelatedEntitiesExistAsync(int serviceOrderId, int partId)
    {
        var serviceOrder = await _dbContext.ServiceOrders.FirstOrDefaultAsync(x => x.Id == serviceOrderId);
        if (serviceOrder is null)
        {
            throw new ArgumentException($"Service order {serviceOrderId} does not exist.");
        }

        if (serviceOrder.ClosedAt is not null)
        {
            throw new InvalidOperationException($"Service order {serviceOrderId} is already closed.");
        }

        var part = await _dbContext.Parts.FirstOrDefaultAsync(x => x.Id == partId && x.IsActive);
        if (part is null)
        {
            throw new ArgumentException($"Part {partId} does not exist or is inactive.");
        }

        return part;
    }

    private async Task EnsurePartIsNotRepeatedAsync(int serviceOrderId, int partId)
    {
        var exists = await _dbContext.ServiceOrderParts.AnyAsync(x =>
            x.ServiceOrderId == serviceOrderId &&
            x.PartId == partId);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Part {partId} is already assigned to service order {serviceOrderId}.");
        }
    }

    private async Task EnsureServiceOrderIsOpenAsync(int serviceOrderId)
    {
        var isClosed = await _dbContext.ServiceOrders
            .Where(x => x.Id == serviceOrderId)
            .Select(x => x.ClosedAt != null)
            .FirstOrDefaultAsync();

        if (isClosed)
        {
            throw new InvalidOperationException($"Service order {serviceOrderId} is already closed.");
        }
    }

    private static ServiceOrderPartDto MapToDto(ServiceOrderPart item)
    {
        return new ServiceOrderPartDto
        {
            Id = item.Id,
            ServiceOrderId = item.ServiceOrderId,
            PartId = item.PartId,
            PartCode = item.Part.Code.Value,
            PartDescription = item.Part.Description.Value,
            Quantity = item.Quantity.Value,
            AppliedUnitPrice = item.AppliedUnitPrice.Value,
            LineTotal = item.Quantity.Value * item.AppliedUnitPrice.Value
        };
    }
}
