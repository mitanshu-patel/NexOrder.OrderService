using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Infrastructure
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OrdersContext>
    {
        public OrdersContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
            var connectionString = configuration.GetConnectionString("SystemDbConnectionString");

            // Explicitly set the migrations assembly
            optionsBuilder.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly("NexOrder.OrderService.Infrastructure")
            );

            return new OrdersContext(optionsBuilder.Options);
        }
    }
}
