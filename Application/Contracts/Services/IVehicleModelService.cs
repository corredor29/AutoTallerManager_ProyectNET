using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.VehicleModels;
using Application.Requests.VehicleModels;

namespace Application.Contracts.Services
{
    public interface IVehicleModelService
    {
        Task<IEnumerable<VehicleModelDto>> GetAllAsync();
        Task<IEnumerable<VehicleModelDto>> GetByBrandIdAsync(int brandId);
        Task<VehicleModelDto?>             GetByIdAsync(int id);
        Task<VehicleModelDto>              CreateAsync(CreateVehicleModelRequest request);
        Task<bool>                         UpdateAsync(int id, UpdateVehicleModelRequest request);
        Task<bool>                         DeleteAsync(int id);
    }
}