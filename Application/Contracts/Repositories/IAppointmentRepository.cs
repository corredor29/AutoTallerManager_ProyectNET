using Domain.Entities.Appointments;

namespace Application.Contracts.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task AddAsync(Appointment appointment);
    void Update(Appointment appointment);
    void Remove(Appointment appointment);
}
