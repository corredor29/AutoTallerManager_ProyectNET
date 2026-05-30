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

    public sealed class FuelTypeRepository : IFuelTypeRepository
    {
        private readonly AutoTallerDbContext _context;

        public FuelTypeRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<FuelType?> GetByIdAsync(int id)
            => await _context.FuelTypes
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<FuelType>> GetAllAsync()
            => await _context.FuelTypes
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.FuelTypes
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(FuelType fuelType)
            => await _context.FuelTypes.AddAsync(fuelType);

        public void Update(FuelType fuelType)
            => _context.FuelTypes.Update(fuelType);

        public void Remove(FuelType fuelType)
            => _context.FuelTypes.Remove(fuelType);
    }
}