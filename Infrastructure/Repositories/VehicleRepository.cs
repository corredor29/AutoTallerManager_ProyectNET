using Application.Contracts.Repositories;
using Domain.Entities.Vehicles;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class VehicleRepository : IVehicleRepository
    {
        private readonly AutoTallerDbContext _dbContext;

        public VehicleRepository(AutoTallerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _dbContext.Vehicles
                .Include(v => v.Model)
                .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _dbContext.Vehicles
                .Include(v => v.Model)
                .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .ToListAsync();
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _dbContext.Vehicles.AddAsync(vehicle);
        }

        public void Update(Vehicle vehicle)
        {
            _dbContext.Vehicles.Update(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            _dbContext.Vehicles.Remove(vehicle);
        }
    }
}
