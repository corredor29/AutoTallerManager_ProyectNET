using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
using Domain.Entities.Vehicles;

    public interface IVehicleBrandRepository
    {
        Task<VehicleBrand?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleBrand>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string brandName);
        Task AddAsync(VehicleBrand vehicleBrand);
        void Update(VehicleBrand vehicleBrand);
        void Remove(VehicleBrand vehicleBrand);
    }
}