using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(int OrderId, OrderStatusCriteria OrderStatusCriteria);
}
