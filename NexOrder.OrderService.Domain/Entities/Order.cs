using NexOrder.OrderService.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public RemoteUser User { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
