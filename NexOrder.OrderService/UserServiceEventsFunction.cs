using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Users.ManageRemoteUser;
using NexOrder.OrderService.Shared.Common;
using NexOrder.UserService.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NexOrder.OrderService
{
    public class UserServiceEventsFunction
    {
        private readonly ILogger<UserServiceEventsFunction> _logger;

        private readonly IMediator mediator;

        public UserServiceEventsFunction(IMediator mediator, ILogger<UserServiceEventsFunction> _logger)
        {
            this.mediator = mediator;
            this._logger = _logger;
        }

        [FunctionName("UserServiceEventsFunction")]
        public void Run([ServiceBusTrigger("userserviceevents", "userserviceorder", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            var response = JsonSerializer.Deserialize<MessageResult>(mySbMsg);

            if (response.FullName == typeof(UserUpdated).FullName)
            {
                var request = JsonSerializer.Deserialize<UserUpdated>(response.Data.ToString());
                this.mediator.SendAsync<ManageRemoteUserCommand, CustomResponse<ManageRemoteUserResult>>(new ManageRemoteUserCommand(request));
                this._logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            }
        }
    }
}
