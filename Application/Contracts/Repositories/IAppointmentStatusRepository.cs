using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Appointments;

namespace Application.Contracts.Repositories
{
    public interface IAppointmentStatusRepository
    {
        Task<AppointmentStatus?>            GetByIdAsync(int id);
        Task<IEnumerable<AppointmentStatus>> GetAllAsync();
        Task<bool>                          ExistsByNameAsync(string name);
        Task                                AddAsync(AppointmentStatus appointmentStatus);
        void                                Update(AppointmentStatus appointmentStatus);
        void                                Remove(AppointmentStatus appointmentStatus);
    }
}