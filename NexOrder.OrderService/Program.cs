using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexOrder.OrderService.Application;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Registrations;
using NexOrder.OrderService.Infrastructure;
using System.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables().Build();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.RegisterHandlers();
builder.Services.AddScoped<IMediator, Mediator>();

builder.Services.AddDbContext<OrdersContext>(
    v => v.UseSqlServer(configuration.GetConnectionString("SystemDbConnectionString"),
    b => b.MigrationsAssembly("NexOrder.OrderService.Infrastructure")));
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Build().Run();
