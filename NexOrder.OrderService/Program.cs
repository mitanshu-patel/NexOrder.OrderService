using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexOrder.Framework.Core;
using NexOrder.Framework.Core.Common;
using NexOrder.OrderService.Application;
using NexOrder.OrderService.Infrastructure;
using NexOrder.OrderService.Infrastructure.Helpers;
using System.Configuration;
using System.Reflection;

var builder = FunctionsApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

var environment = configuration.GetValue<string>("ENVIRONMENT");
var isDevelopment = !string.IsNullOrEmpty(environment) && environment.Equals(
            "DEVELOPMENT",
            System.StringComparison.InvariantCultureIgnoreCase);

builder.ConfigureFunctionsWebApplication();

var appInsightsConnection = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

builder.Services.AddNexOrderCustomLogging(isDevelopment, "NexOrder.OrderService", appInsightsConnection);
builder.Services.RegisterHandlers(Assembly.Load("NexOrder.OrderService.Application"));


builder.Services.AddDbContext<OrdersContext>(
    v => v.UseSqlServer(ConnectionStringsHelper.GetDbConnectionString(),
    b => b.MigrationsAssembly("NexOrder.OrderService.Infrastructure")));
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
var app = builder.Build();
if (builder.Configuration.GetValue<bool>("RunMigration"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrdersContext>();
    db.Database.Migrate();
}

app.Run();
