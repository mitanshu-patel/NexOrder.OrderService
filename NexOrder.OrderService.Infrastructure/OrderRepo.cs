using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexOrder.OrderService.Application;
using NexOrder.OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Infrastructure
{
    public class OrderRepo : IOrderRepo
    {
        private readonly OrdersContext dbContext;

        public OrderRepo(OrdersContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddRemoteProductAsync(RemoteProduct remoteProduct)
        {
            this.dbContext.RemoteProducts.Add(remoteProduct);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task AddRemoteUserAsync(RemoteUser remoteUser)
        {
            this.dbContext.RemoteUsers.Add(remoteUser);
            await this.dbContext.SaveChangesAsync();
        }

        public IQueryable<Order> GetOrders()
        {
            return dbContext.Orders.AsQueryable();
        }

        public IQueryable<ProductStock> GetProductStocks()
        {
            return dbContext.ProductStocks.AsQueryable();
        }

        public IQueryable<RemoteProduct> GetRemoteProducts()
        {
            return dbContext.RemoteProducts.AsQueryable();
        }

        public IQueryable<RemoteUser> GetRemoteUsers()
        {
            return dbContext.RemoteUsers.AsQueryable();
        }

        public async Task SaveOrderAsync(Order order)
        {
            if(order.Id > 0)
            {
                dbContext.Orders.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                dbContext.Orders.Add(order);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateProductStockAsync(int productStockId, int requestedQuantity)
        {
            // We use this raw sql query to make sure the AvailableQuantity is updated atomically in the database.
            return await dbContext.Database.ExecuteSqlRawAsync(@"UPDATE ProductStocks SET AvailableQuantity = AvailableQuantity - {0}, LastUpdatedAtUtc = {1} WHERE Id = {2} AND AvailableQuantity >= {0} AND AvailableQuantity - {0} >= 0",
                requestedQuantity, DateTime.UtcNow, productStockId);
        }

        public async Task SaveProductStockAsync(ProductStock productStock)
        {
            if (productStock.Id > 0)
            {
                dbContext.ProductStocks.Entry(productStock).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                dbContext.ProductStocks.Add(productStock);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRemoteProductAsync(RemoteProduct remoteProduct)
        {
            this.dbContext.RemoteProducts.Entry(remoteProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task UpdateRemoteUserAsync(RemoteUser remoteUser)
        {
            this.dbContext.RemoteUsers.Entry(remoteUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
    }
}
