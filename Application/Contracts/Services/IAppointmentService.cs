using Application.DTOs.Appointments;
using Application.Requests.Appointments;

namespace Application.Contracts.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentRequest request);
    Task<bool> UpdateAsync(int id, UpdateAppointmentRequest request);
    Task<bool> ChangeStatusAsync(int id, ChangeAppointmentStatusRequest request);
    Task<bool> DeleteAsync(int id);
}
