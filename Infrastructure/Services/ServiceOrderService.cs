using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.ServiceOrders;
using Application.Filters;
using Application.Requests.ServiceOrders;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.ServiceOrders.ServiceOrder;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class ServiceOrderService : IServiceOrderService
{
    private static readonly string[] ClosingStatuses   = ["completed", "cancelled", "canceled"];
    private static readonly string[] CancelledStatuses = ["cancelled", "canceled"];

    private readonly IServiceOrderRepository _serviceOrderRepository;
    private readonly AutoTallerDbContext     _dbContext;

    public ServiceOrderService(IServiceOrderRepository serviceOrderRepository, AutoTallerDbContext dbContext)
    {
        _serviceOrderRepository = serviceOrderRepository;
        _dbContext              = dbContext;
    }

    public async Task<PagedResult<ServiceOrderDto>> GetAllPagedAsync(
        PaginationParams pagination, ServiceOrderFilter filter)
    {
        var result = await _serviceOrderRepository.GetAllPagedAsync(pagination, filter);
        return new PagedResult<ServiceOrderDto>
        {
            Items      = result.Items.Select(MapToDto).ToArray(),
            PageNumber = result.PageNumber,
            PageSize   = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<ServiceOrderDto?> GetByIdAsync(int id)
    {
        var order = await _serviceOrderRepository.GetByIdAsync(id);
        return order is null ? null : MapToDto(order);
    }

    public async Task<ServiceOrderDto> CreateAsync(CreateServiceOrderRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(
            request.VehicleId,
            request.ServiceTypeId,
            request.MechanicId,
            request.OrderStatusId,
            request.AppointmentId);

        if (await _serviceOrderRepository.HasActiveOrderForVehicleAsync(request.VehicleId))
            throw new InvalidOperationException(
                $"Vehicle {request.VehicleId} already has an active service order (Pending or In Progress).");

        var estimatedDeliveryAt = request.EstimatedDeliveryAt;
        if (!estimatedDeliveryAt.HasValue)
        {
            var serviceType   = await _dbContext.ServiceTypes.FindAsync(request.ServiceTypeId);
            var durationHours = serviceType?.EstimatedDuration.Value ?? 2;
            estimatedDeliveryAt = DateTime.UtcNow.AddHours(durationHours);
        }

        var order = new ServiceOrder(
            request.VehicleId,
            request.ServiceTypeId,
            request.MechanicId,
            request.OrderStatusId,
            new WorkDescription(request.WorkPerformed),
            new ServiceOrderNotes(request.Notes),
            request.AppointmentId,
            estimatedDeliveryAt);

        if (await ShouldCloseOrderAsync(request.OrderStatusId))
            order.Close();

        await _serviceOrderRepository.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var createdOrder = await _serviceOrderRepository.GetByIdAsync(order.Id)
            ?? throw new InvalidOperationException("Service order could not be reloaded after creation.");

        return MapToDto(createdOrder);
    }

    public async Task<bool> UpdateAsync(int id, UpdateServiceOrderRequest request)
    {
        var order = await _serviceOrderRepository.GetByIdAsync(id);
        if (order is null) return false;

        EnsureOrderIsOpen(order);

        order.Update(
            new WorkDescription(request.WorkPerformed),
            new ServiceOrderNotes(request.Notes),
            request.EstimatedDeliveryAt);

        _serviceOrderRepository.Update(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangeServiceOrderStatusRequest request)
    {
        var order = await _serviceOrderRepository.GetByIdAsync(id);
        if (order is null) return false;

        EnsureOrderIsOpen(order);

        if (!await _dbContext.OrderStatuses.AnyAsync(x => x.Id == request.OrderStatusId))
            throw new ArgumentException($"Order status {request.OrderStatusId} does not exist.");

        if (await ShouldReleaseReservedPartsAsync(request.OrderStatusId))
            await ReleaseReservedPartsAsync(order.Id);

        order.ChangeStatus(request.OrderStatusId);

        if (await ShouldCloseOrderAsync(request.OrderStatusId) && order.ClosedAt is null)
            order.Close();

        _serviceOrderRepository.Update(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _serviceOrderRepository.GetByIdAsync(id);
        if (order is null) return false;

        await ReleaseReservedPartsAsync(order.Id);

        _serviceOrderRepository.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(
        int vehicleId, int serviceTypeId, int mechanicId,
        int orderStatusId, int? appointmentId)
    {
        if (!await _dbContext.Vehicles.AnyAsync(x => x.Id == vehicleId))
            throw new ArgumentException($"Vehicle {vehicleId} does not exist.");

        if (!await _dbContext.ServiceTypes.AnyAsync(x => x.Id == serviceTypeId))
            throw new ArgumentException($"Service type {serviceTypeId} does not exist.");

        if (!await _dbContext.Users.AnyAsync(x => x.Id == mechanicId && x.IsActive))
            throw new ArgumentException($"Mechanic user {mechanicId} does not exist or is inactive.");

        if (!await _dbContext.OrderStatuses.AnyAsync(x => x.Id == orderStatusId))
            throw new ArgumentException($"Order status {orderStatusId} does not exist.");

        if (appointmentId.HasValue &&
            !await _dbContext.Appointments.AnyAsync(x => x.Id == appointmentId.Value))
            throw new ArgumentException($"Appointment {appointmentId.Value} does not exist.");
    }

    // ── Fix: ToListAsync + filtro en memoria ───────
    private async Task<bool> ShouldCloseOrderAsync(int orderStatusId)
    {
        var all        = await _dbContext.OrderStatuses.ToListAsync();
        var statusName = all.FirstOrDefault(x => x.Id == orderStatusId)?.Name.Value.ToLower();
        return statusName is not null && ClosingStatuses.Contains(statusName);
    }

    private async Task<bool> ShouldReleaseReservedPartsAsync(int orderStatusId)
    {
        var all        = await _dbContext.OrderStatuses.ToListAsync();
        var statusName = all.FirstOrDefault(x => x.Id == orderStatusId)?.Name.Value.ToLower();
        return statusName is not null && CancelledStatuses.Contains(statusName);
    }

    private async Task ReleaseReservedPartsAsync(int serviceOrderId)
    {
        var reservedParts = await _dbContext.ServiceOrderParts
            .Include(x => x.Part)
            .Where(x => x.ServiceOrderId == serviceOrderId)
            .ToListAsync();

        if (reservedParts.Count == 0) return;

        foreach (var reservedPart in reservedParts)
            reservedPart.Part.AddStock(
                new Domain.ValueObject.Parts.Part.PartStock(reservedPart.Quantity.Value));

        _dbContext.ServiceOrderParts.RemoveRange(reservedParts);
    }

    private static void EnsureOrderIsOpen(ServiceOrder order)
    {
        if (order.ClosedAt is not null)
            throw new InvalidOperationException($"Service order {order.Id} is already closed.");
    }

    private static ServiceOrderDto MapToDto(ServiceOrder order)
    {
        var mechanicFullName = order.Mechanic?.Person is null
            ? string.Empty
            : $"{order.Mechanic.Person.FirstName.Value} {order.Mechanic.Person.LastName.Value}".Trim();

        var vehicleDisplayName = order.Vehicle?.Model is null
            ? string.Empty
            : $"{order.Vehicle.Model.Brand.BrandName.Value} {order.Vehicle.Model.ModelName.Value}".Trim();

        return new ServiceOrderDto
        {
            Id                  = order.Id,
            VehicleId           = order.VehicleId,
            VehicleVin          = order.Vehicle?.VIN.Value          ?? string.Empty,
            VehicleDisplayName  = vehicleDisplayName,
            ServiceTypeId       = order.ServiceTypeId,
            ServiceTypeName     = order.ServiceType?.Name.Value      ?? string.Empty,
            MechanicId          = order.MechanicId,
            MechanicName        = mechanicFullName,
            OrderStatusId       = order.OrderStatusId,
            OrderStatusName     = order.OrderStatus?.Name.Value      ?? string.Empty,
            AppointmentId       = order.AppointmentId,
            CreatedAt           = order.CreatedAt,
            EstimatedDeliveryAt = order.EstimatedDeliveryAt,
            ClosedAt            = order.ClosedAt,
            WorkPerformed       = order.WorkPerformed.Value,
            Notes               = order.Notes.Value
        };
    }
}