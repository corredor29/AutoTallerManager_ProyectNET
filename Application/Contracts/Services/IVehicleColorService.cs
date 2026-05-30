using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.VehicleColors;
using Application.Requests.VehicleColors;

namespace Application.Contracts.Services
{
    public interface IVehicleColorService
    {
        Task<IEnumerable<VehicleColorDto>> GetAllAsync();
        Task<VehicleColorDto?>             GetByIdAsync(int id);
        Task<VehicleColorDto>              CreateAsync(CreateVehicleColorRequest request);
        Task<bool>                         UpdateAsync(int id, UpdateVehicleColorRequest request);
        Task<bool>                         DeleteAsync(int id);
    }
}