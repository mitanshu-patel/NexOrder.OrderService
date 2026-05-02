using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.Framework.Core.Common;
using NexOrder.OrderService.Domain.Entities;
using NexOrder.OrderService.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.CreateOrder
{
    public class CreateOrderHandler : RequestHandlerBase<CreateOrderCommand, CustomResponse<CreateOrderResult>>
    {
        private readonly ILogger<CreateOrderHandler> logger;
        
        private readonly IOrderRepo orderRepo;
        public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }
        protected override async Task<CustomResponse<CreateOrderResult>> ExecuteCommandAsync(CreateOrderCommand command)
        {
           try
            {
                this.logger.LogDebug("CreateOrderHandler: Creating order for UserId:{userId} with {itemCount} items.", command.Criteria.UserId, command.Criteria.OrderItems.Count);
                var productIds = command.Criteria.OrderItems.Select(v => v.ProductId).ToList();
                var productStockDetails = await this.orderRepo.GetProductStocks()
                                    .Where(v => productIds.Contains(v.ProductId))
                                    .Select(v=> new { ProductStock = v, Price = v.Product.Price})
                                    .ToListAsync();

                // We use this idempotency key check to prevent duplicate orders in case of retries with same key.
                var orderExistWithIdempotencyKey = await this.orderRepo.GetOrders().AnyAsync(v => v.IdempotencyKey == command.IdempotencyKey);
                var validationBuilder = ValidationErrorBuilder.Create();

                if(orderExistWithIdempotencyKey)
                {
                    validationBuilder.AddObjectError("An order already exists with same Idempotency key");
                }

                var newOrder = new Order();
                newOrder.UserId = command.Criteria.UserId;
                newOrder.CreatedAtUtc = DateTime.UtcNow;
                newOrder.Status = OrderStatus.Confirmed;
                newOrder.IdempotencyKey = command.IdempotencyKey;
                var totalAmount = 0m;
                foreach (var item in command.Criteria.OrderItems)
                {
                    var productStocks = productStockDetails.Select(v => v.ProductStock);
                    var productStock = productStocks.FirstOrDefault(p => p.ProductId == item.ProductId);
                    var updatedQuantity = productStock != null ? productStock.AvailableQuantity - item.Quantity : -1;
                    var productIndex = command.Criteria.OrderItems.IndexOf(item);

                    if (productStock == null)
                    {
                        this.logger.LogWarning("CreateOrderHandler: ProductId:{productId} not found in stock.", item.ProductId);
                        validationBuilder.AddPropertyError($"OrderItems[{productIndex}].ProductId", "Product not found in stock.");
                        continue;
                    }

                    if (updatedQuantity < 0)
                    {
                        this.logger.LogWarning("CreateOrderHandler: Insufficient stock for ProductId:{productId}. Requested:{requested}, Available:{available}", item.ProductId, item.Quantity, productStock?.AvailableQuantity ?? 0);
                        validationBuilder.AddPropertyError($"OrderItems[{productIndex}].ProductId", $"Insufficient stock. Requested:{item.Quantity}, Available:{productStock?.AvailableQuantity ?? 0}");
                        continue;
                    }

                    var unitPrice = productStockDetails.FirstOrDefault(v => v.ProductStock.ProductId == item.ProductId)?.Price ?? 0;
                    totalAmount += (unitPrice * item.Quantity);
                    productStock!.AvailableQuantity = updatedQuantity;
                    newOrder.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice
                    });
                }

                if(validationBuilder.HasErrors())
                {
                    this.logger.LogWarning("CreateOrderHandler: Validation failed while creating order for UserId:{userId}", command.Criteria.UserId);
                    return validationBuilder.Build<CreateOrderResult>();
                }

                newOrder.TotalAmount = totalAmount;
                await this.orderRepo.SaveOrderAsync(newOrder);

                this.logger.LogDebug("CreateOrderHandler: Successfully created order with Id:{orderId} for UserId:{userId}", newOrder.Id, command.Criteria.UserId);
                return CustomHttpResult.Ok(new CreateOrderResult(newOrder.Id));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "CreateOrderHandler: Error occurred while creating order with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<CreateOrderResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<CreateOrderResult>();
        }

        protected override IValidator<CreateOrderCommand> GetValidator()
        {
            var validator = new InlineValidator<CreateOrderCommand>();
            validator.RuleFor(v => v.Criteria).NotNull();
            validator.RuleFor(v => v.Criteria.OrderItems).NotEmpty().WithMessage("Order must contain at least one item.");
            validator.RuleFor(v => v.Criteria.UserId).GreaterThan(0).WithMessage("UserId must be greater than zero.");
            validator.RuleFor(v => v.Criteria.OrderItems.All(t => t.ProductId > 0)).NotEmpty().WithMessage("All order items must have a valid ProductId greater than zero.");
            validator.RuleFor(v => v.Criteria.OrderItems.All(t => t.Quantity > 0)).NotEmpty().WithMessage("All order items must have a Quantity greater than zero.");
            return validator;
        }
    }
}
