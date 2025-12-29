using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SaveProductStock
{
    public record ProductStockCriteria(int ProductId, int AvailableQuantity);
}
