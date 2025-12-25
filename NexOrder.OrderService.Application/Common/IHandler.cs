using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Common
{
    public interface IHandler<TCommand, TResult> where TCommand : class
    {
        Task<TResult> Handle(TCommand command);
    }
}
