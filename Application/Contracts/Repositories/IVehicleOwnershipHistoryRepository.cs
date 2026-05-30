using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IVehicleOwnershipHistoryRepository
    {
        Task<VehicleOwnershipHistory?>            GetByIdAsync(int id);
        Task<IEnumerable<VehicleOwnershipHistory>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleOwnershipHistory>> GetByCustomerIdAsync(int customerId);
        Task<VehicleOwnershipHistory?>            GetCurrentOwnerAsync(int vehicleId);
        Task<bool>                                HasActiveOwnershipAsync(int vehicleId);
        Task                                      AddAsync(VehicleOwnershipHistory ownership);
        void                                      Update(VehicleOwnershipHistory ownership);
    }
}