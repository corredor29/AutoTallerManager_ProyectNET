using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.MileageHistory;
using Application.Requests.MileageHistory;

namespace Application.Contracts.Services
{
    public interface IMileageHistoryService
    {
        Task<IEnumerable<MileageHistoryDto>> GetByVehicleIdAsync(int vehicleId);
        Task<MileageHistoryDto?>             GetLatestByVehicleIdAsync(int vehicleId);
        Task<MileageHistoryDto?>             GetByIdAsync(int id);
        Task<MileageHistoryDto>              CreateAsync(CreateMileageHistoryRequest request);
        Task<bool>                           UpdateAsync(int id, UpdateMileageHistoryRequest request);
        Task<bool>                           DeleteAsync(int id);
    }
}