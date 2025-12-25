using NexOrder.ProductService.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Products.ManageRemoteProduct
{
    public record ManageRemoteProductCommand(ProductUpdated Message);
}
