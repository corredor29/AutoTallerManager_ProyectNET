using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.OrderStatuses;
using Application.Requests.OrderStatuses;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.ServiceOrders.OrderStatus;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly AutoTallerDbContext     _dbContext;

        public OrderStatusService(IOrderStatusRepository orderStatusRepository,
                                AutoTallerDbContext dbContext)
        {
            _orderStatusRepository = orderStatusRepository;
            _dbContext             = dbContext;
        }

        public async Task<IEnumerable<OrderStatusDto>> GetAllAsync()
        {
            var statuses = await _orderStatusRepository.GetAllAsync();
            return statuses.Select(MapToDto);
        }

        public async Task<OrderStatusDto?> GetByIdAsync(int id)
        {
            var status = await _orderStatusRepository.GetByIdAsync(id);
            return status is null ? null : MapToDto(status);
        }

        public async Task<OrderStatusDto> CreateAsync(CreateOrderStatusRequest request)
        {
            await EnsureNameIsUniqueAsync(request.Name);

            var status = new OrderStatus(new OrderStatusName(request.Name));

            await _orderStatusRepository.AddAsync(status);
            await _dbContext.SaveChangesAsync();

            return MapToDto(status);
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderStatusRequest request)
        {
            var status = await _orderStatusRepository.GetByIdAsync(id);
            if (status is null) return false;

            await EnsureNameIsUniqueAsync(request.Name, id);

            status.Update(new OrderStatusName(request.Name));

            _orderStatusRepository.Update(status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var status = await _orderStatusRepository.GetByIdAsync(id);
            if (status is null) return false;

            if (await _dbContext.ServiceOrders.AnyAsync(x => x.OrderStatusId == id))
                throw new InvalidOperationException($"Order status {id} is being used and cannot be deleted.");

            _orderStatusRepository.Remove(status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.OrderStatuses.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Order status '{name}' already exists.");
        }

        private static OrderStatusDto MapToDto(OrderStatus status) => new()
        {
            Id   = status.Id,
            Name = status.Name.Value
        };
    }
}