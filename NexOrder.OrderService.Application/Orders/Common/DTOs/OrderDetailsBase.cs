using NexOrder.OrderService.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.Common.DTOs
{
    public record OrderDetailsBase
    {
        public int OrderId { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }

        public decimal TotalAmount { get; init; }

        public OrderStatus Status { get; init; }

        public DateTime CreatedAtUtc { get; init; }
    }
}
