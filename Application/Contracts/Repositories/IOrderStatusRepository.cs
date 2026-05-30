using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.ServiceOrders;

namespace Application.Contracts.Repositories
{
    public interface IOrderStatusRepository
    {
        Task<OrderStatus?>            GetByIdAsync(int id);
        Task<IEnumerable<OrderStatus>> GetAllAsync();
        Task<bool>                    ExistsByNameAsync(string name);
        Task                          AddAsync(OrderStatus orderStatus);
        void                          Update(OrderStatus orderStatus);
        void                          Remove(OrderStatus orderStatus);
    }
}