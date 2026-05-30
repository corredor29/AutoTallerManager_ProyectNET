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
    public sealed class VehicleBrandRepository : IVehicleBrandRepository
    {
        private readonly AutoTallerDbContext _context;

        public VehicleBrandRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleBrand?> GetByIdAsync(int id)
            => await _context.VehicleBrands
                            .Include(x => x.Models)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<VehicleBrand>> GetAllAsync()
            => await _context.VehicleBrands
                            .Include(x => x.Models)
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string brandName)
            => await _context.VehicleBrands
                            .AnyAsync(x => x.BrandName.Value == brandName);

        public async Task AddAsync(VehicleBrand vehicleBrand)
            => await _context.VehicleBrands.AddAsync(vehicleBrand);

        public void Update(VehicleBrand vehicleBrand)
            => _context.VehicleBrands.Update(vehicleBrand);

        public void Remove(VehicleBrand vehicleBrand)
            => _context.VehicleBrands.Remove(vehicleBrand);
    }
}