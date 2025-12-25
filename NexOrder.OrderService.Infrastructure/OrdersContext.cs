using Microsoft.EntityFrameworkCore;
using NexOrder.OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Infrastructure
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
        {
        }

        public DbSet<RemoteUser> RemoteUsers { get; set; }

        public DbSet<RemoteProduct> RemoteProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(OrdersContext).Assembly);
        }
    }
}
