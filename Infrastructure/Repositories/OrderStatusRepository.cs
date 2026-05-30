using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.ServiceOrders;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly AutoTallerDbContext _context;

        public OrderStatusRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<OrderStatus?> GetByIdAsync(int id)
            => await _context.OrderStatuses
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<OrderStatus>> GetAllAsync()
            => await _context.OrderStatuses
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.OrderStatuses
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(OrderStatus orderStatus)
            => await _context.OrderStatuses.AddAsync(orderStatus);

        public void Update(OrderStatus orderStatus)
            => _context.OrderStatuses.Update(orderStatus);

        public void Remove(OrderStatus orderStatus)
            => _context.OrderStatuses.Remove(orderStatus);
    }
}