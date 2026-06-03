using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Appointments;
using Application.DTOs.Notifications;
using Application.Requests.Appointments;
using Domain.Entities.Appointments;
using Domain.ValueObject.Appointments.Appointment;
using Infrastructure.Context;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class AppointmentService : IAppointmentService
{
    private static readonly string[] ActiveAppointmentStatuses = ["pending", "confirmed"];

    private readonly IAppointmentRepository      _appointmentRepository;
    private readonly AutoTallerDbContext          _dbContext;
    private readonly IHubContext<NotificationHub> _hub;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        AutoTallerDbContext dbContext,
        IHubContext<NotificationHub> hub)
    {
        _appointmentRepository = appointmentRepository;
        _dbContext             = dbContext;
        _hub                   = hub;
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        return appointment is null ? null : MapToDto(appointment);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(
            request.CustomerId, request.VehicleId,
            request.ServiceTypeId, request.AppointmentStatusId,
            request.AssignedUserId);

        await EnsureMechanicAvailabilityAsync(
            request.AssignedUserId,
            request.AppointmentDate,
            request.ServiceTypeId);

        var appointment = new Appointment(
            request.CustomerId, request.VehicleId,
            request.ServiceTypeId, request.AppointmentStatusId,
            new AppointmentDate(request.AppointmentDate),
            new AppointmentNotes(request.Notes),
            request.AssignedUserId);

        await _appointmentRepository.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        var createdAppointment = await _appointmentRepository.GetByIdAsync(appointment.Id)
            ?? throw new InvalidOperationException("Appointment could not be reloaded after creation.");

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "create",
            Entity     = "Appointment",
            RecordId   = appointment.Id,
            Message    = $"New appointment #A-{appointment.Id} scheduled",
            OccurredAt = DateTime.UtcNow
        });

        return MapToDto(createdAppointment);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAppointmentRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return false;

        if (request.AssignedUserId.HasValue &&
            !await _dbContext.Users.AnyAsync(x => x.Id == request.AssignedUserId.Value && x.IsActive))
            throw new ArgumentException($"Assigned user {request.AssignedUserId.Value} does not exist or is inactive.");

        await EnsureMechanicAvailabilityAsync(
            request.AssignedUserId,
            request.AppointmentDate,
            appointment.ServiceTypeId,
            appointment.Id);

        appointment.Update(
            new AppointmentDate(request.AppointmentDate),
            new AppointmentNotes(request.Notes),
            request.AssignedUserId);

        _appointmentRepository.Update(appointment);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "update",
            Entity     = "Appointment",
            RecordId   = id,
            Message    = $"Appointment #A-{id} updated",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangeAppointmentStatusRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return false;

        if (!await _dbContext.AppointmentStatuses.AnyAsync(x => x.Id == request.AppointmentStatusId))
            throw new ArgumentException($"Appointment status {request.AppointmentStatusId} does not exist.");

        appointment.ChangeStatus(request.AppointmentStatusId);
        _appointmentRepository.Update(appointment);
        await _dbContext.SaveChangesAsync();

        var allStatuses = await _dbContext.AppointmentStatuses.ToListAsync();
        var statusName  = allStatuses.FirstOrDefault(x => x.Id == request.AppointmentStatusId)?.Name.Value ?? "Unknown";

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "status",
            Entity     = "Appointment",
            RecordId   = id,
            Message    = $"Appointment #A-{id} status changed to {statusName}",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return false;

        if (await _dbContext.ServiceOrders.AnyAsync(x => x.AppointmentId == id))
            throw new InvalidOperationException(
                $"Appointment {id} is being used by a service order and cannot be deleted.");

        _appointmentRepository.Remove(appointment);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "delete",
            Entity     = "Appointment",
            RecordId   = id,
            Message    = $"Appointment #A-{id} deleted",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(
        int customerId, int vehicleId, int serviceTypeId,
        int appointmentStatusId, int? assignedUserId)
    {
        if (!await _dbContext.Customers.AnyAsync(x => x.Id == customerId))
            throw new ArgumentException($"Customer {customerId} does not exist.");

        if (!await _dbContext.Vehicles.AnyAsync(x => x.Id == vehicleId))
            throw new ArgumentException($"Vehicle {vehicleId} does not exist.");

        if (!await _dbContext.ServiceTypes.AnyAsync(x => x.Id == serviceTypeId))
            throw new ArgumentException($"Service type {serviceTypeId} does not exist.");

        if (!await _dbContext.AppointmentStatuses.AnyAsync(x => x.Id == appointmentStatusId))
            throw new ArgumentException($"Appointment status {appointmentStatusId} does not exist.");

        if (assignedUserId.HasValue &&
            !await _dbContext.Users.AnyAsync(x => x.Id == assignedUserId.Value && x.IsActive))
            throw new ArgumentException($"Assigned user {assignedUserId.Value} does not exist or is inactive.");
    }

    private async Task EnsureMechanicAvailabilityAsync(
        int? assignedUserId, DateTime appointmentDate,
        int serviceTypeId, int? excludeAppointmentId = null)
    {
        if (!assignedUserId.HasValue) return;

        var serviceTypes  = await _dbContext.ServiceTypes.ToListAsync();
        var st            = serviceTypes.FirstOrDefault(x => x.Id == serviceTypeId);
        var durationHours = st?.EstimatedDuration?.Value ?? 1;

        var startAt = appointmentDate;
        var endAt   = appointmentDate.AddHours(durationHours > 0 ? durationHours : 1);

        var activeStatuses  = await _dbContext.AppointmentStatuses.ToListAsync();
        var activeStatusIds = activeStatuses
            .Where(s => ActiveAppointmentStatuses.Contains(s.Name.Value.ToLower()))
            .Select(s => s.Id)
            .ToList();

        var appointments = await _dbContext.Appointments
            .Include(x => x.ServiceType)
            .Where(x => x.AssignedUserId == assignedUserId.Value)
            .Where(x => !excludeAppointmentId.HasValue || x.Id != excludeAppointmentId.Value)
            .Where(x => activeStatusIds.Contains(x.AppointmentStatusId))
            .ToListAsync();

        var hasConflict = appointments.Any(x =>
            x.AppointmentDate.Value < endAt &&
            x.AppointmentDate.Value.AddHours(x.ServiceType?.EstimatedDuration?.Value ?? 1) > startAt);

        if (hasConflict)
            throw new InvalidOperationException(
                $"Mechanic {assignedUserId.Value} already has another appointment scheduled in that time range.");

        var serviceOrders = await _dbContext.ServiceOrders
            .Include(x => x.ServiceType)
            .Where(x => x.MechanicId == assignedUserId.Value && x.ClosedAt == null)
            .ToListAsync();

        var hasSOConflict = serviceOrders.Any(x =>
            x.CreatedAt < endAt &&
            (x.EstimatedDeliveryAt ?? x.CreatedAt.AddHours(x.ServiceType?.EstimatedDuration?.Value ?? 1)) > startAt);

        if (hasSOConflict)
            throw new InvalidOperationException(
                $"Mechanic {assignedUserId.Value} already has a service order in progress for that time range.");
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        var customerName = appointment.Customer?.Person is null
            ? string.Empty
            : $"{appointment.Customer.Person.FirstName.Value} {appointment.Customer.Person.LastName.Value}".Trim();

        var assignedUserName = appointment.AssignedUser?.Person is null
            ? string.Empty
            : $"{appointment.AssignedUser.Person.FirstName.Value} {appointment.AssignedUser.Person.LastName.Value}".Trim();

        var vehicleDisplayName = appointment.Vehicle?.Model is null
            ? string.Empty
            : $"{appointment.Vehicle.Model.Brand.BrandName.Value} {appointment.Vehicle.Model.ModelName.Value}".Trim();

        return new AppointmentDto
        {
            Id                    = appointment.Id,
            CustomerId            = appointment.CustomerId,
            CustomerName          = customerName,
            VehicleId             = appointment.VehicleId,
            VehicleVin            = appointment.Vehicle?.VIN.Value           ?? string.Empty,
            VehicleDisplayName    = vehicleDisplayName,
            ServiceTypeId         = appointment.ServiceTypeId,
            ServiceTypeName       = appointment.ServiceType?.Name.Value       ?? string.Empty,
            AppointmentStatusId   = appointment.AppointmentStatusId,
            AppointmentStatusName = appointment.AppointmentStatus?.Name.Value ?? string.Empty,
            AssignedUserId        = appointment.AssignedUserId,
            AssignedUserName      = assignedUserName,
            AppointmentDate       = appointment.AppointmentDate.Value,
            Notes                 = appointment.Notes.Value
        };
    }
}