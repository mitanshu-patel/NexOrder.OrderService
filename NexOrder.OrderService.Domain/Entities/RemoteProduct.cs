using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Domain.Entities
{
    /// <summary>
    /// This is named as Remote product because this entity is mastered in NexOrder.ProductService.
    /// So whenever product details are needed, they will be fetched from NexOrder.ProductService and will be stored in this entity.
    /// </summary>
    public class RemoteProduct
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public ProductStock? ProductStock { get; set; }

        public List<OrderItem> OrderItems { get; set; } = [];
    }
}
