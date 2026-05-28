using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task AddAsync(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Remove(Vehicle vehicle);
    }
}
