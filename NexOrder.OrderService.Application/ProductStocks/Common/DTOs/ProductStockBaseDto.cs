using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.Common.DTOs
{
    public record ProductStockBaseDto(int Id, string ProductName, decimal Price, int AvailableQuantity);
}
