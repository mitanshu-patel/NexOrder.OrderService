using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OrderIdempotencyAttribute : Attribute
    {
    }
}
