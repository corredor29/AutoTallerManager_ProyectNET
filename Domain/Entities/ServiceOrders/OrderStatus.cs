using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.ServiceOrders.OrderStatus;
namespace Domain.Entities.ServiceOrders
{

    public sealed class OrderStatus : BaseEntity
    {
        public OrderStatusName Name { get; private set; } = null!;

        public ICollection<ServiceOrder> ServiceOrders { get; private set; } = [];

        private OrderStatus() { }

        public OrderStatus(OrderStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(OrderStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}