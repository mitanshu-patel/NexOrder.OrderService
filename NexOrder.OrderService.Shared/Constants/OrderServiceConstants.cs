using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Shared.Constants
{
    public static class OrderServiceConstants
    {
        public static string IdempotencyKeyHeader => "Idempotency-Key";
    }
}
