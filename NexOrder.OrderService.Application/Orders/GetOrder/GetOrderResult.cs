using NexOrder.OrderService.Application.Orders.GetOrder.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.GetOrder
{
    public record GetOrderResult(OrderDetailsDto OrderDetails);
}
