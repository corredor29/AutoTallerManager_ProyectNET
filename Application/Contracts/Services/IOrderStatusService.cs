using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.OrderStatuses;
using Application.Requests.OrderStatuses;

namespace Application.Contracts.Services
{
    public interface IOrderStatusService
    {
        Task<IEnumerable<OrderStatusDto>> GetAllAsync();
        Task<OrderStatusDto?>             GetByIdAsync(int id);
        Task<OrderStatusDto>              CreateAsync(CreateOrderStatusRequest request);
        Task<bool>                        UpdateAsync(int id, UpdateOrderStatusRequest request);
        Task<bool>                        DeleteAsync(int id);
    }
}