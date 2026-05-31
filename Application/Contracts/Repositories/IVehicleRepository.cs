using Application.Common.Pagination;
using Application.Filters;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IVehicleRepository
    {
        Task<PagedResult<Vehicle>> GetAllPagedAsync(PaginationParams pagination, VehicleFilter filter);
        Task<Vehicle?> GetByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<bool> ExistsByVinAsync(string vin, int? excludeId = null);
        Task AddAsync(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Remove(Vehicle vehicle);
    }
}
