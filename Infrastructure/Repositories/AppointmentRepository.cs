using Application.Contracts.Repositories;
using Domain.Entities.Appointments;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public AppointmentRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _dbContext.Appointments
            .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model!)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.ServiceType)
            .Include(x => x.AppointmentStatus)
            .Include(x => x.AssignedUser)
                .ThenInclude(x => x!.Person)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _dbContext.Appointments
            .Include(x => x.Customer)
                .ThenInclude(x => x.Person)
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model!)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.ServiceType)
            .Include(x => x.AppointmentStatus)
            .Include(x => x.AssignedUser)
                .ThenInclude(x => x!.Person)
            .OrderBy(x => x.AppointmentDate)
            .ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _dbContext.Appointments.AddAsync(appointment);
    }

    public void Update(Appointment appointment)
    {
        _dbContext.Appointments.Update(appointment);
    }

    public void Remove(Appointment appointment)
    {
        _dbContext.Appointments.Remove(appointment);
    }
}
