using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SaveProductStock
{
    public class SaveProductStockHandler : RequestHandlerBase<SaveProductStockCommand, CustomResponse<SaveProductStockResult>>
    {
        private readonly ILogger<SaveProductStockHandler> logger;
        private readonly IOrderRepo orderRepo;
        public SaveProductStockHandler(ILogger<SaveProductStockHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }
        protected override async Task<CustomResponse<SaveProductStockResult>> ExecuteCommandAsync(SaveProductStockCommand command)
        {
            try
            {
                this.logger.LogDebug("SaveProductStockHandler : Saving product stock for ProductId:{productId}", command.ProductStockCriteria.ProductId);
                var productStockId = command.ProductStockId ?? 0;
                if (command.ProductStockId.HasValue)
                {
                    var productStock = await this.orderRepo.GetProductStocks().Where(v => v.Id == command.ProductStockId).FirstOrDefaultAsync();
                    if(productStock == null)
                    {
                        return CustomHttpResult.NotFound<SaveProductStockResult>("Product stock details not found");
                    }

                    this.logger.LogDebug("SaveProductStockHandler : Updating product stock for ProductStockId:{productStockId}", command.ProductStockId);
                    productStock.AvailableQuantity = command.ProductStockCriteria.AvailableQuantity;
                    productStock.LastUpdatedAtUtc = DateTime.UtcNow;
                    await this.orderRepo.SaveProductStockAsync(productStock);
                    this.logger.LogDebug("Updated product stock for ProductStockId:{productStockId}", command.ProductStockId);
                }
                else
                {
                    this.logger.LogDebug("SaveProductStockHandler : Creating new product stock for ProductId:{productId}", command.ProductStockCriteria.ProductId);

                    var productStockExists = await this.orderRepo.GetProductStocks()
                        .AnyAsync(v => v.ProductId == command.ProductStockCriteria.ProductId);

                    if (productStockExists)
                    {
                        this.logger.LogWarning("SaveProductStockHandler : Product stock already exists for ProductId:{productId}", command.ProductStockCriteria.ProductId);
                        return CustomHttpResult.BadRequest<SaveProductStockResult>("Product stock already exists for the given ProductId.");
                    }

                    var newProductStock = new Domain.Entities.ProductStock
                    {
                        ProductId = command.ProductStockCriteria.ProductId,
                        AvailableQuantity = command.ProductStockCriteria.AvailableQuantity,
                        LastUpdatedAtUtc = DateTime.UtcNow
                    };
                    await this.orderRepo.SaveProductStockAsync(newProductStock);
                    productStockId = newProductStock.Id;
                    this.logger.LogDebug("SaveProductStockHandler : Created new product stock with ProductStockId:{productStockId}", newProductStock.Id);
                }

                this.logger.LogDebug("SaveProductStockHandler : Saved product stock for ProductStockId:{productStockId}", productStockId);
                return CustomHttpResult.Ok(new SaveProductStockResult(productStockId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "SaveProductStockHandler : Error occurred while saving product stock with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<SaveProductStockResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<SaveProductStockResult>();
        }

        protected override IValidator<SaveProductStockCommand> GetValidator()
        {
            var validator = new InlineValidator<SaveProductStockCommand>();
            validator.RuleFor(v => v.ProductStockCriteria).NotNull().WithMessage("ProductStockCriteria is required.");
            validator.RuleFor(v=>v.ProductStockCriteria.ProductId).GreaterThan(0).WithMessage("ProductId must be greater than 0.");
            validator.RuleFor(v=>v.ProductStockCriteria.AvailableQuantity).GreaterThanOrEqualTo(0).WithMessage("AvailableQuantity must be greater than or equal to 0.");
            return validator;
        }
    }
}
