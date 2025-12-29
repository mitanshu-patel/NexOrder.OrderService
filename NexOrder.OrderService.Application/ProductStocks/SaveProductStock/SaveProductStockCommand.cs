using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SaveProductStock
{
    public record SaveProductStockCommand(ProductStockCriteria ProductStockCriteria, int? ProductStockId = null);
}
