using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.ProductStocks.Common.DTOs;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.ProductStocks.SearchProductStocks
{
    public class SearchProductStocksHandler : RequestHandlerBase<SearchProductStocksQuery, CustomResponse<SearchProductStocksResult>>
    {
        private readonly ILogger<SearchProductStocksHandler> logger;

        private readonly IOrderRepo orderRepo;
        public SearchProductStocksHandler(ILogger<SearchProductStocksHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }

        protected override async Task<CustomResponse<SearchProductStocksResult>> ExecuteCommandAsync(SearchProductStocksQuery command)
        {
            try
            {
                this.logger.LogInformation("SearchProductStocksHandler: ExecuteCommandAsync execution started");
                var productStocks = this.orderRepo.GetProductStocks();

                if (!string.IsNullOrEmpty(command.SearchText))
                {
                    productStocks = productStocks.Where(v => v.Product.Name.Contains(command.SearchText) || v.Product.Name.Contains(command.SearchText));
                }

                var totalRecords = await productStocks.CountAsync();

                var productStocksList = await productStocks
                                .OrderByDescending(v => v.LastUpdatedAtUtc)
                                .Select(v => new ProductStockBaseDto(v.Id, v.Product.Name, v.Product.Price, v.AvailableQuantity))
                                .Skip(command.PageIndex * command.PageSize)
                                .Take(command.PageSize)
                                .ToListAsync();

                this.logger.LogInformation("SearchProductStocksHandler: ExecuteCommandAsync execution completed and found {count} productStocks", totalRecords);

                return CustomHttpResult.Ok(new SearchProductStocksResult(productStocksList, totalRecords));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "SearchProductStocksHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<SearchProductStocksResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<SearchProductStocksResult>();
        }

        protected override IValidator<SearchProductStocksQuery> GetValidator()
        {
            var validator = new InlineValidator<SearchProductStocksQuery>();
            validator.RuleFor(v => v.PageIndex).GreaterThanOrEqualTo(0);
            validator.RuleFor(v => v.PageSize).GreaterThan(0);
            return validator;
        }
    }
}
