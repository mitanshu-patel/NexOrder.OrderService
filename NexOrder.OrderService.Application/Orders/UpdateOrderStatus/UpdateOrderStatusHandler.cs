using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.Framework.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : RequestHandlerBase<UpdateOrderStatusCommand, CustomResponse<UpdateOrderStatusResult>>
    {
        private readonly ILogger<UpdateOrderStatusHandler> logger;
        
        private readonly IOrderRepo orderRepo;
        public UpdateOrderStatusHandler(ILogger<UpdateOrderStatusHandler> logger, IOrderRepo orderRepo)
        {
            this.orderRepo = orderRepo;
            this.logger = logger;
        }
        protected override async Task<CustomResponse<UpdateOrderStatusResult>> ExecuteCommandAsync(UpdateOrderStatusCommand command)
        {
            try
            {
                this.logger.LogDebug("UpdateOrderStatusHandler: Updating order status for OrderId: {OrderId} to Status: {OrderStatus}", command.OrderId, command.OrderStatusCriteria.OrderStatus);
                var order = await this.orderRepo.GetOrders().Where(v=>v.Id == command.OrderId).FirstOrDefaultAsync();
                if(order == null)
                {
                    this.logger.LogWarning("UpdateOrderStatusHandler: Order not found for OrderId: {OrderId}", command.OrderId);
                    return CustomHttpResult.NotFound<UpdateOrderStatusResult>($"Order with Id {command.OrderId} not found.");
                }

                order.Status = command.OrderStatusCriteria.OrderStatus;

                await this.orderRepo.SaveOrderAsync(order);

                this.logger.LogDebug("UpdateOrderStatusHandler: Successfully updated order status for OrderId: {OrderId} to Status: {OrderStatus}", command.OrderId, command.OrderStatusCriteria.OrderStatus);

                return CustomHttpResult.Ok(new UpdateOrderStatusResult());
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "UpdateOrderStatusHandler: Error updating order status for OrderId: {OrderId} with message:{message}", command.OrderId, ex.Message);
                throw;
            }
        }

        protected override CustomResponse<UpdateOrderStatusResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<UpdateOrderStatusResult>();
        }

        protected override IValidator<UpdateOrderStatusCommand> GetValidator()
        {
            var validator = new InlineValidator<UpdateOrderStatusCommand>();
            validator.RuleFor(x => x.OrderId)
                     .GreaterThan(0).WithMessage("OrderId must be greater than zero.");
            validator.RuleFor(v => v.OrderStatusCriteria.OrderStatus)
                     .IsInEnum().WithMessage("OrderStatusCriteria must be a valid enum value.");

            return validator;
        }
    }
}
