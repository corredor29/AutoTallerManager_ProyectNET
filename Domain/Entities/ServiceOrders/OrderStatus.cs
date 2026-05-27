using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.ServiceOrders
{
    public class OrderStatus : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = [];
    }
}