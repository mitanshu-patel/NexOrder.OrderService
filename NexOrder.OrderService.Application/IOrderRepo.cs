using NexOrder.OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application
{
    public interface IOrderRepo
    {
        public IQueryable<Order> GetOrders();

        public Task SaveOrderAsync(Order order);

        public Task AddRemoteProductAsync(RemoteProduct remoteProduct);

        public Task UpdateRemoteProductAsync(RemoteProduct remoteProduct);

        public Task AddRemoteUserAsync(RemoteUser remoteUser);

        public Task UpdateRemoteUserAsync(RemoteUser remoteUser);

        public IQueryable<RemoteProduct> GetRemoteProducts();
        public IQueryable<RemoteUser> GetRemoteUsers();

        public Task SaveProductStockAsync(ProductStock productStock);
    }
}
