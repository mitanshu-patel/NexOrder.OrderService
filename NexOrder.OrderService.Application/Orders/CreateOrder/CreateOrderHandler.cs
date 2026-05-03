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
            using var transaction = await this.orderRepo.BeginTransactionAsync();
            try
            {
                this.logger.LogDebug("CreateOrderHandler: Creating order for UserId:{userId} with {itemCount} items.", command.Criteria.UserId, command.Criteria.OrderItems.Count);
                var productIds = command.Criteria.OrderItems.Select(v => v.ProductId).ToList();
                var productStockDetails = await this.orderRepo.GetProductStocks()
                                    .Where(v => productIds.Contains(v.ProductId) && v.AvailableQuantity > 0)
                                    .Select(v=> new { ProductId = v.ProductId, ProductStockId = v.Id, Price = v.Product.Price})
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
                    var productStockId = productStockDetails.FirstOrDefault(v => v.ProductId == item.ProductId)?.ProductStockId;
                    var rowsAffected = await this.orderRepo.UpdateProductStockAsync(productStockId ?? 0, item.Quantity);
                    var productIndex = command.Criteria.OrderItems.IndexOf(item);

                    if (rowsAffected == 0)
                    {
                        // We rollback the transaction and return validation error if any of the products in the order do not have sufficient stock to fulfill the order.
                        // This is to ensure that we do not end up in a scenario where some of the products in the order are reserved while others are not, which can lead to a bad customer experience.
                        this.logger.LogWarning("CreateOrderHandler: Insufficient stock for ProductId:{productId}. Requested:{requested}", item.ProductId, item.Quantity);
                        validationBuilder.AddPropertyError($"OrderItems[{productIndex}].ProductId", $"Cannot place order for requested product as product is out of stock.");
                        await transaction.RollbackAsync();
                        return validationBuilder.Build<CreateOrderResult>();
                    }
                    
                    var unitPrice = productStockDetails.FirstOrDefault(v => v.ProductId == item.ProductId)?.Price ?? 0;
                    totalAmount += (unitPrice * item.Quantity);
                    
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

                await transaction.CommitAsync();

                this.logger.LogDebug("CreateOrderHandler: Successfully created order with Id:{orderId} for UserId:{userId}", newOrder.Id, command.Criteria.UserId);
                return CustomHttpResult.Ok(new CreateOrderResult(newOrder.Id));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "CreateOrderHandler: Error occurred while creating order with message:{message}", ex.Message);
                await transaction.RollbackAsync();
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
            validator.RuleFor(v => v.Criteria.OrderItems.GroupBy(t => t.ProductId).Any(g => g.Count() > 1)).Equal(false).WithMessage("Duplicate ProductIds are not allowed in order items.");
            validator.RuleFor(v => v.Criteria.OrderItems.All(t => t.ProductId > 0)).NotEmpty().WithMessage("All order items must have a valid ProductId greater than zero.");
            validator.RuleFor(v => v.Criteria.OrderItems.All(t => t.Quantity > 0)).NotEmpty().WithMessage("All order items must have a Quantity greater than zero.");
            return validator;
        }
    }
}
