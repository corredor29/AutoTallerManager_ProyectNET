using Application.DTOs.Vehicles;
using Application.Requests.Vehicles;

namespace Application.Contracts.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllAsync();
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<VehicleDto> CreateAsync(CreateVehicleRequest request);
        Task<bool> UpdateAsync(int id, UpdateVehicleRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
