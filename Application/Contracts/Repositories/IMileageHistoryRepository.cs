using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IMileageHistoryRepository
    {
        Task<MileageHistory?>            GetByIdAsync(int id);
        Task<IEnumerable<MileageHistory>> GetByVehicleIdAsync(int vehicleId);
        Task<MileageHistory?>            GetLatestByVehicleIdAsync(int vehicleId);
        Task                             AddAsync(MileageHistory mileageHistory);
        void                             Update(MileageHistory mileageHistory);
        void                             Remove(MileageHistory mileageHistory);
    }
}