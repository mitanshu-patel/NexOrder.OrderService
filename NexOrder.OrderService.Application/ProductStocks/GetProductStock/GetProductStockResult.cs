using NexOrder.OrderService.Application.ProductStocks.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.GetProductStock
{
    public record GetProductStockResult(ProductStockBaseDto StockDetails);
}
