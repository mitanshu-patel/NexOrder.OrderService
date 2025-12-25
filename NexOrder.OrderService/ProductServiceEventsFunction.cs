using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Products.ManageRemoteProduct;
using NexOrder.OrderService.Shared.Common;
using NexOrder.ProductService.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NexOrder.OrderService
{
    public class ProductServiceEventsFunction
    {
        private readonly ILogger<ProductServiceEventsFunction> _logger;

        private readonly IMediator mediator;

        public ProductServiceEventsFunction(IMediator mediator, ILogger<ProductServiceEventsFunction> _logger)
        {
            this.mediator = mediator;
            this._logger = _logger;
        }

        [FunctionName("ProductServiceEventsFunction")]
        public void Run([ServiceBusTrigger("productserviceevents", "productserviceorder", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var response = JsonSerializer.Deserialize<MessageResult>(mySbMsg);
            if (response.FullName == typeof(ProductUpdated).FullName)
            {
                var request = JsonSerializer.Deserialize<ProductUpdated>(response.Data.ToString());
                this.mediator.SendAsync<ManageRemoteProductCommand, CustomResponse<ManageRemoteProductResult>>(new ManageRemoteProductCommand(request));
                this._logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            }
        }
    }
}
