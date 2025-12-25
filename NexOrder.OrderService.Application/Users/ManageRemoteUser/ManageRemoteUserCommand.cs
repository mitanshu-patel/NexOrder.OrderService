using NexOrder.UserService.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Users.ManageRemoteUser
{
    public record ManageRemoteUserCommand(UserUpdated Message);
}
