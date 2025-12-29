using NexOrder.OrderService.Application.ProductStocks.SearchProductStocks.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SearchProductStocks
{
    public record SearchProductStocksResult(List<ProductStocksDto> ProductStocks, int TotalRecords);
}
