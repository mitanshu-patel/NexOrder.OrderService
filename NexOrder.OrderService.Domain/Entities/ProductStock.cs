using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Domain.Entities
{
    public class ProductStock
    {
        public int Id { get; set; }

        public int AvailableQuantity { get; set; }

        public int ProductId { get; set; }

        public RemoteProduct Product { get; set; }

        public DateTime LastUpdatedAtUtc { get; set; }
    }
}
