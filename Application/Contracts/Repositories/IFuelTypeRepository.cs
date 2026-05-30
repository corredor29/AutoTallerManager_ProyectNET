using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IFuelTypeRepository
    {
        Task<FuelType?>            GetByIdAsync(int id);
        Task<IEnumerable<FuelType>> GetAllAsync();
        Task<bool>                 ExistsByNameAsync(string name);
        Task                       AddAsync(FuelType fuelType);
        void                       Update(FuelType fuelType);
        void                       Remove(FuelType fuelType);
    }
}