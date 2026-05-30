using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.AppointmentStatuses;
using Application.Requests.AppointmentStatuses;
using Domain.Entities.Appointments;
using Domain.ValueObject.Appointments.AppointmentStatus;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class AppointmentStatusService : IAppointmentStatusService
    {
        private readonly IAppointmentStatusRepository _appointmentStatusRepository;
        private readonly AutoTallerDbContext           _dbContext;

        public AppointmentStatusService(IAppointmentStatusRepository appointmentStatusRepository,
                                        AutoTallerDbContext dbContext)
        {
            _appointmentStatusRepository = appointmentStatusRepository;
            _dbContext                   = dbContext;
        }

        public async Task<IEnumerable<AppointmentStatusDto>> GetAllAsync()
        {
            var statuses = await _appointmentStatusRepository.GetAllAsync();
            return statuses.Select(MapToDto);
        }

        public async Task<AppointmentStatusDto?> GetByIdAsync(int id)
        {
            var status = await _appointmentStatusRepository.GetByIdAsync(id);
            return status is null ? null : MapToDto(status);
        }

        public async Task<AppointmentStatusDto> CreateAsync(CreateAppointmentStatusRequest request)
        {
            await EnsureNameIsUniqueAsync(request.Name);

            var status = new AppointmentStatus(
                new AppointmentStatusName(request.Name)
            );

            await _appointmentStatusRepository.AddAsync(status);
            await _dbContext.SaveChangesAsync();

            return MapToDto(status);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAppointmentStatusRequest request)
        {
            var status = await _appointmentStatusRepository.GetByIdAsync(id);
            if (status is null) return false;

            await EnsureNameIsUniqueAsync(request.Name, id);

            status.Update(new AppointmentStatusName(request.Name));

            _appointmentStatusRepository.Update(status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var status = await _appointmentStatusRepository.GetByIdAsync(id);
            if (status is null) return false;

            if (await _dbContext.Appointments.AnyAsync(x => x.AppointmentStatusId == id))
                throw new InvalidOperationException($"Appointment status {id} is being used and cannot be deleted.");

            _appointmentStatusRepository.Remove(status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.AppointmentStatuses.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Appointment status '{name}' already exists.");
        }

        private static AppointmentStatusDto MapToDto(AppointmentStatus status) => new()
        {
            Id   = status.Id,
            Name = status.Name.Value
        };
    }
}