using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.VehicleBrands;
using Application.Requests.VehicleBrands;

namespace Application.Contracts.Services
{
    public interface IVehicleBrandService
    {
        Task<IEnumerable<VehicleBrandDto>> GetAllAsync();
        Task<VehicleBrandDto?> GetByIdAsync(int id);
        Task<VehicleBrandDto> CreateAsync(CreateVehicleBrandRequest request);
        Task<bool> UpdateAsync(int id, UpdateVehicleBrandRequest request);
        Task<bool>  DeleteAsync(int id);
    }
}