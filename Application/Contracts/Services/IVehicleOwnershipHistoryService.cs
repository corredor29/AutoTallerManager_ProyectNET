using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.VehicleOwnershipHistory;
using Application.Requests.VehicleOwnershipHistory;

namespace Application.Contracts.Services
{
    public interface IVehicleOwnershipHistoryService
    {
        Task<IEnumerable<VehicleOwnershipHistoryDto>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleOwnershipHistoryDto>> GetByCustomerIdAsync(int customerId);
        Task<VehicleOwnershipHistoryDto?>             GetCurrentOwnerAsync(int vehicleId);
        Task<VehicleOwnershipHistoryDto>              CreateAsync(CreateOwnershipRequest request);
        Task<bool>                                    CloseAsync(int id, CloseOwnershipRequest request);
    }
}