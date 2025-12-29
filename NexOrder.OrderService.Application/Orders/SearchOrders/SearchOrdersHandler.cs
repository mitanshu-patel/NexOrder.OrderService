using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Orders.SearchOrders.DTOs;
using NexOrder.OrderService.Application.ProductStocks.SearchProductStocks;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Application.Orders.SearchOrders
{
    public class SearchOrdersHandler : RequestHandlerBase<SearchOrdersQuery, CustomResponse<SearchOrdersResult>>
    {
        private readonly ILogger<SearchOrdersHandler> logger;

        private readonly IOrderRepo orderRepo;
        public SearchOrdersHandler(ILogger<SearchOrdersHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }
        protected override async Task<CustomResponse<SearchOrdersResult>> ExecuteCommandAsync(SearchOrdersQuery command)
        {
            try
            {
                this.logger.LogInformation("SearchOrdersHandler: ExecuteCommandAsync execution started");
                var orders = this.orderRepo.GetOrders();

                if (!string.IsNullOrEmpty(command.SearchText))
                {
                    orders = orders.Where(v => v.User.Name.Contains(command.SearchText) || v.User.Email.Contains(command.SearchText));
                }

                var totalRecords = await orders.CountAsync();

                var ordersList = await orders
                                .OrderByDescending(v => v.CreatedAtUtc)
                                .Select(v => new OrdersDto
                                {
                                    CreatedAtUtc = v.CreatedAtUtc,
                                    OrderId = v.Id,
                                    Email = v.User.Email,
                                    Name = v.User.Name,
                                    TotalAmount = v.TotalAmount,
                                    Status = v.Status,
                                })
                                .Skip(command.PageIndex * command.PageSize)
                                .Take(command.PageSize)
                                .ToListAsync();

                this.logger.LogInformation("SearchOrdersHandler: ExecuteCommandAsync execution completed and found {count} orders", totalRecords);

                return CustomHttpResult.Ok(new SearchOrdersResult(ordersList, totalRecords));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "SearchOrdersHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<SearchOrdersResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<SearchOrdersResult>();
        }

        protected override IValidator<SearchOrdersQuery> GetValidator()
        {
            var validator = new InlineValidator<SearchOrdersQuery>();
            validator.RuleFor(v => v.PageIndex).GreaterThanOrEqualTo(0);
            validator.RuleFor(v => v.PageSize).GreaterThan(0);
            return validator;
        }
    }
}
