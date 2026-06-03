using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Notifications;
using Application.DTOs.Parts;
using Application.Filters;
using Application.Requests.Parts;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.Part;
using Infrastructure.Context;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PartService : IPartService
{
    private readonly IPartRepository             _partRepository;
    private readonly AutoTallerDbContext          _dbContext;
    private readonly IHubContext<NotificationHub> _hub;

    public PartService(
        IPartRepository partRepository,
        AutoTallerDbContext dbContext,
        IHubContext<NotificationHub> hub)
    {
        _partRepository = partRepository;
        _dbContext      = dbContext;
        _hub            = hub;
    }

    public async Task<PagedResult<PartDto>> GetAllPagedAsync(
        PaginationParams pagination, PartFilter filter)
    {
        var result = await _partRepository.GetAllPagedAsync(pagination, filter);
        return new PagedResult<PartDto>
        {
            Items      = result.Items.Select(MapToDto).ToArray(),
            PageNumber = result.PageNumber,
            PageSize   = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<PartDto?> GetByIdAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);
        return part is null ? null : MapToDto(part);
    }

    public async Task<PartDto> CreateAsync(CreatePartRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.PartCategoryId, request.UnitId);
        await EnsureCodeIsUniqueAsync(request.Code);

        var part = new Part(
            request.PartCategoryId,
            new PartCode(request.Code),
            new PartDescription(request.Description),
            new PartStock(request.Stock),
            new PartMinStock(request.MinStock),
            new PartUnitPrice(request.UnitPrice),
            request.UnitId);

        await _partRepository.AddAsync(part);
        await _dbContext.SaveChangesAsync();

        var createdPart = await _partRepository.GetByIdAsync(part.Id)
            ?? throw new InvalidOperationException("Part could not be reloaded after creation.");

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "create",
            Entity     = "Part",
            RecordId   = part.Id,
            Message    = $"New part '{request.Code}' added to inventory",
            OccurredAt = DateTime.UtcNow
        });

        return MapToDto(createdPart);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePartRequest request)
    {
        var part = await _partRepository.GetByIdAsync(id);
        if (part is null) return false;

        await EnsureRelatedEntitiesExistAsync(request.PartCategoryId, request.UnitId);
        await EnsureCodeIsUniqueAsync(request.Code, id);

        part.Update(
            request.PartCategoryId,
            new PartCode(request.Code),
            new PartDescription(request.Description),
            new PartStock(request.Stock),
            new PartMinStock(request.MinStock),
            new PartUnitPrice(request.UnitPrice),
            request.UnitId,
            request.IsActive);

        _partRepository.Update(part);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "update",
            Entity     = "Part",
            RecordId   = id,
            Message    = $"Part '{request.Code}' updated",
            OccurredAt = DateTime.UtcNow
        });

        // Notificar si stock bajo
        if (request.Stock <= request.MinStock)
        {
            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "warning",
                Entity     = "Part",
                RecordId   = id,
                Message    = $" Part '{request.Code}' is below minimum stock ({request.Stock}/{request.MinStock})",
                OccurredAt = DateTime.UtcNow
            });
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);
        if (part is null) return false;

        if (await _dbContext.QuotationDetails.AnyAsync(x => x.PartId == id) ||
            await _dbContext.ServiceOrderParts.AnyAsync(x => x.PartId == id))
            throw new InvalidOperationException($"Part {id} is being used and cannot be deleted.");

        _partRepository.Remove(part);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "delete",
            Entity     = "Part",
            RecordId   = id,
            Message    = $"Part #{id} deleted from inventory",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int partCategoryId, int? unitId)
    {
        if (!await _dbContext.PartCategories.AnyAsync(x => x.Id == partCategoryId))
            throw new ArgumentException($"Part category {partCategoryId} does not exist.");

        if (unitId.HasValue && !await _dbContext.MeasurementUnits.AnyAsync(x => x.Id == unitId.Value))
            throw new ArgumentException($"Measurement unit {unitId.Value} does not exist.");
    }

    private async Task EnsureCodeIsUniqueAsync(string code, int? excludeId = null)
    {
        var normalizedCode = code.Trim().ToUpper();
        var all    = await _dbContext.Parts.ToListAsync();
        var exists = all.Any(x =>
            x.Code.Value.ToUpper() == normalizedCode &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
            throw new InvalidOperationException($"Part code '{code}' already exists.");
    }

    private static PartDto MapToDto(Part part) => new()
    {
        Id               = part.Id,
        PartCategoryId   = part.PartCategoryId,
        PartCategoryName = part.Category?.Name.Value    ?? string.Empty,
        UnitId           = part.UnitId,
        UnitName         = part.Unit?.Name.Value,
        UnitAbbreviation = part.Unit?.Abbreviation.Value,
        Code             = part.Code?.Value              ?? string.Empty,
        Description      = part.Description?.Value       ?? string.Empty,
        Stock            = part.Stock?.Value             ?? 0,
        MinStock         = part.MinStock?.Value          ?? 0,
        UnitPrice        = part.UnitPrice?.Value         ?? 0,
        IsActive         = part.IsActive
    };
}