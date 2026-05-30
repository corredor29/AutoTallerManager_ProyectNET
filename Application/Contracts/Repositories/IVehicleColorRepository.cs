using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IVehicleColorRepository
    {
        Task<VehicleColor?>            GetByIdAsync(int id);
        Task<IEnumerable<VehicleColor>> GetAllAsync();
        Task<bool>                     ExistsByNameAsync(string name);
        Task                           AddAsync(VehicleColor vehicleColor);
        void                           Update(VehicleColor vehicleColor);
        void                           Remove(VehicleColor vehicleColor);
    }
}