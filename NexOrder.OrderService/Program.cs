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
using NexOrder.OrderService.Infrastructure.Helpers;
using System.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.RegisterHandlers();
builder.Services.AddScoped<IMediator, Mediator>();

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
