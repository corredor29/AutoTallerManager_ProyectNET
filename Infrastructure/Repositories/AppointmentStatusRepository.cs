using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Appointments;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class AppointmentStatusRepository : IAppointmentStatusRepository
    {
        private readonly AutoTallerDbContext _context;

        public AppointmentStatusRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentStatus?> GetByIdAsync(int id)
            => await _context.AppointmentStatuses
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<AppointmentStatus>> GetAllAsync()
            => await _context.AppointmentStatuses
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.AppointmentStatuses
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(AppointmentStatus appointmentStatus)
            => await _context.AppointmentStatuses.AddAsync(appointmentStatus);

        public void Update(AppointmentStatus appointmentStatus)
            => _context.AppointmentStatuses.Update(appointmentStatus);

        public void Remove(AppointmentStatus appointmentStatus)
            => _context.AppointmentStatuses.Remove(appointmentStatus);
    }
}