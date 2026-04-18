using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.Framework.Core.Common;
using NexOrder.OrderService.Application.Orders.GetOrder.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.GetOrder
{
    public class GetOrderHandler : RequestHandlerBase<GetOrderQuery, CustomResponse<GetOrderResult>>
    {
        private readonly ILogger<GetOrderHandler> logger;

        private readonly IOrderRepo orderRepo;
        public GetOrderHandler(ILogger<GetOrderHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }
        protected override async Task<CustomResponse<GetOrderResult>> ExecuteCommandAsync(GetOrderQuery command)
        {
            try
            {
                this.logger.LogDebug("GetOrderHandler: Initializing handler for OrderId:{orderId}", command.OrderId);
                var order = await this.orderRepo.GetOrders().Where(v => v.Id == command.OrderId).Select(v => new OrderDetailsDto
                {
                    CreatedAtUtc = v.CreatedAtUtc,
                    TotalAmount = v.TotalAmount,
                    Email = v.User.Email,
                    Name = v.User.Name,
                    OrderId = v.Id,
                    Status = v.Status,
                    OrderItems = v.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        ProductId = oi.ProductId,
                    }).ToList(),
                }).FirstOrDefaultAsync();

                if(order == null)
                {
                    this.logger.LogWarning("GetOrderHandler: Order not found for OrderId:{orderId}", command.OrderId);
                    return CustomHttpResult.NotFound<GetOrderResult>($"Order with Id {command.OrderId} not found.");
                }

                this.logger.LogDebug("GetOrderHandler: Successfully retrieved order for OrderId:{orderId}", command.OrderId);
                return CustomHttpResult.Ok(new GetOrderResult(order));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "GetOrderHandler: Exception occurred during handler initialization with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<GetOrderResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<GetOrderResult>();
        }

        protected override IValidator<GetOrderQuery> GetValidator()
        {
            var validator = new InlineValidator<GetOrderQuery>();
            validator.RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("OrderId must be greater than zero.");
            return validator;
        }
    }
}
