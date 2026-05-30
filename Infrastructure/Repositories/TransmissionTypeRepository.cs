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
    public sealed class TransmissionTypeRepository : ITransmissionTypeRepository
    {
        private readonly AutoTallerDbContext _context;

        public TransmissionTypeRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<TransmissionType?> GetByIdAsync(int id)
            => await _context.TransmissionTypes
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<TransmissionType>> GetAllAsync()
            => await _context.TransmissionTypes
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.TransmissionTypes
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(TransmissionType transmissionType)
            => await _context.TransmissionTypes.AddAsync(transmissionType);

        public void Update(TransmissionType transmissionType)
            => _context.TransmissionTypes.Update(transmissionType);

        public void Remove(TransmissionType transmissionType)
            => _context.TransmissionTypes.Remove(transmissionType);
    }
}