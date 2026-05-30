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

    public sealed class VehicleColorRepository : IVehicleColorRepository
    {
        private readonly AutoTallerDbContext _context;

        public VehicleColorRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleColor?> GetByIdAsync(int id)
            => await _context.VehicleColors
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<VehicleColor>> GetAllAsync()
            => await _context.VehicleColors
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.VehicleColors
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(VehicleColor vehicleColor)
            => await _context.VehicleColors.AddAsync(vehicleColor);

        public void Update(VehicleColor vehicleColor)
            => _context.VehicleColors.Update(vehicleColor);

        public void Remove(VehicleColor vehicleColor)
            => _context.VehicleColors.Remove(vehicleColor);
    }
}