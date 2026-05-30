using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AppointmentStatuses;
using Application.Requests.AppointmentStatuses;

namespace Application.Contracts.Services
{
    public interface IAppointmentStatusService
    {
        Task<IEnumerable<AppointmentStatusDto>> GetAllAsync();
        Task<AppointmentStatusDto?>             GetByIdAsync(int id);
        Task<AppointmentStatusDto>              CreateAsync(CreateAppointmentStatusRequest request);
        Task<bool>                              UpdateAsync(int id, UpdateAppointmentStatusRequest request);
        Task<bool>                              DeleteAsync(int id);
    }
}