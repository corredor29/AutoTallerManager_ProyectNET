using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Vehicles;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class VehicleModelRepository : IVehicleModelRepository
    {
        private readonly AutoTallerDbContext _context;

        public VehicleModelRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleModel?> GetByIdAsync(int id)
            => await _context.VehicleModels
                            .Include(x => x.Brand)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<VehicleModel>> GetAllAsync()
            => await _context.VehicleModels
                            .Include(x => x.Brand)
                            .ToListAsync();

        public async Task<IEnumerable<VehicleModel>> GetByBrandIdAsync(int brandId)
            => await _context.VehicleModels
                            .Include(x => x.Brand)
                            .Where(x => x.BrandId == brandId)
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(int brandId, string modelName)
            => await _context.VehicleModels
                            .AnyAsync(x => x.BrandId == brandId
                                        && x.ModelName.Value == modelName);

        public async Task AddAsync(VehicleModel vehicleModel)
            => await _context.VehicleModels.AddAsync(vehicleModel);

        public void Update(VehicleModel vehicleModel)
            => _context.VehicleModels.Update(vehicleModel);

        public void Remove(VehicleModel vehicleModel)
            => _context.VehicleModels.Remove(vehicleModel);
    }
}