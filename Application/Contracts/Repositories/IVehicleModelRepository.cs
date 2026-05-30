using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IVehicleModelRepository
    {
        Task<VehicleModel?>            GetByIdAsync(int id);
        Task<IEnumerable<VehicleModel>> GetAllAsync();
        Task<IEnumerable<VehicleModel>> GetByBrandIdAsync(int brandId);
        Task<bool>                     ExistsByNameAsync(int brandId, string modelName);
        Task                           AddAsync(VehicleModel vehicleModel);
        void                           Update(VehicleModel vehicleModel);
        void                           Remove(VehicleModel vehicleModel);
    }
}