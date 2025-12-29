using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SearchProductStocks.DTOs
{
    public record ProductStocksDto(int Id, string ProductName, decimal Price, int AvailableQuantity);
}
