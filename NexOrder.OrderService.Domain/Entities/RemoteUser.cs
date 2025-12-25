using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Domain.Entities
{
    public class RemoteUser
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<Order> Orders { get; set; } = [];
    }
}
