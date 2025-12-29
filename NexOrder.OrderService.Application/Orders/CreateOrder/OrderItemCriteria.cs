using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.CreateOrder
{
    public record OrderItemCriteria(int ProductId, int Quantity);
}
