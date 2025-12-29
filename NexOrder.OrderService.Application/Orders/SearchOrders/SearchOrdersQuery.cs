using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.SearchOrders
{
    public record SearchOrdersQuery(int PageNumber, int PageSize, string? SearchText = null)
    {
        [JsonIgnore]
        public int PageIndex { get => this.PageNumber == 0 ? this.PageNumber : this.PageNumber - 1; }
    }
}
