using NexOrder.OrderService.Application.Orders.Common.DTOs;
using NexOrder.OrderService.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.GetOrder.DTOs
{
    public record OrderDetailsDto: OrderDetailsBase
    {
        public List<OrderItemDto> OrderItems { get; init; }
    }
}
