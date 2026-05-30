using Application.Common.Pagination;
using Application.DTOs.Vehicles;
using Application.Requests.Vehicles;

namespace Application.Contracts.Services
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleDto>> GetAllAsync(GetVehiclesRequest request);
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<VehicleDto> CreateAsync(CreateVehicleRequest request);
        Task<bool> UpdateAsync(int id, UpdateVehicleRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
