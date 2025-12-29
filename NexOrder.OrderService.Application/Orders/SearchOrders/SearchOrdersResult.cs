using NexOrder.OrderService.Application.Orders.SearchOrders.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.SearchOrders
{
    public record SearchOrdersResult(List<OrdersDto> Orders, int TotalRecords);
}
