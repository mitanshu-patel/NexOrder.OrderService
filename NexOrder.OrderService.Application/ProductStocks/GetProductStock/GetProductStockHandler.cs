using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Orders.GetOrder;
using NexOrder.OrderService.Application.Orders.GetOrder.DTOs;
using NexOrder.OrderService.Application.ProductStocks.Common.DTOs;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.GetProductStock
{
    public class GetProductStockHandler : RequestHandlerBase<GetProductStockQuery, CustomResponse<GetProductStockResult>>
    {
        private readonly ILogger<GetProductStockHandler> logger;

        private readonly IOrderRepo orderRepo;
        public GetProductStockHandler(ILogger<GetProductStockHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }

        protected override async Task<CustomResponse<GetProductStockResult>> ExecuteCommandAsync(GetProductStockQuery command)
        {
            try
            {
                this.logger.LogDebug("GetProductStockHandler: Initializing handler for ProductStockId:{id}", command.Id);
                var productStock = await this.orderRepo.GetProductStocks()
                    .Where(v => v.Id == command.Id)
                      .Select(v => new ProductStockBaseDto(v.Id, v.Product.Name, v.Product.Price, v.AvailableQuantity))
                      .FirstOrDefaultAsync();

                if (productStock == null)
                {
                    this.logger.LogWarning("GetProductStockHandler: Stock details not found for ProductStockId:{id}", command.Id);
                    return CustomHttpResult.NotFound<GetProductStockResult>($"ProductStock with Id {command.Id} not found.");
                }

                this.logger.LogDebug("GetProductStockHandler: Successfully retrieved stock details for ProductStockId:{id}", command.Id);
                return CustomHttpResult.Ok(new GetProductStockResult(productStock));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "GetProductStockHandler: Exception occurred during handler initialization with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<GetProductStockResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<GetProductStockResult>();
        }

        protected override IValidator<GetProductStockQuery> GetValidator()
        {
            var validator = new InlineValidator<GetProductStockQuery>();
            validator.RuleFor(x => x.Id).GreaterThan(0).WithMessage("ProductStockId must be greater than zero.");
            return validator;
        }
    }
}
