using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NexOrder.OrderService.Application.Products.ManageRemoteProduct
{
    public class ManageRemoteProductHandler : RequestHandlerBase<ManageRemoteProductCommand, CustomResponse<ManageRemoteProductResult>>
    {
        private readonly IOrderRepo orderRepo;
        private readonly ILogger<ManageRemoteProductHandler> logger;

        public ManageRemoteProductHandler(IOrderRepo orderRepo, ILogger<ManageRemoteProductHandler> logger)
        {
            this.orderRepo = orderRepo;
            this.logger = logger;
        }

        protected override async Task<CustomResponse<ManageRemoteProductResult>> ExecuteCommandAsync(ManageRemoteProductCommand command)
        {
            try
            {
                this.logger.LogDebug("ManageRemoteProductHandler: Managing remote product with ID {ProductId}", command.Message.Id);
                var product = await this.orderRepo.GetRemoteProducts().Where(v=>v.Id == command.Message.Id).FirstOrDefaultAsync();
                if (product == null)
                {
                    this.logger.LogDebug("ManageRemoteProductHandler: Remote product with ID {ProductId} not found. Creating new product.", command.Message.Id);

                    await this.orderRepo.AddRemoteProductAsync(new Domain.Entities.RemoteProduct
                    {
                        Id = command.Message.Id,
                        Name = command.Message.Name,
                        Description = command.Message.Description,
                        Price = command.Message.Price
                    });
                    
                    this.logger.LogDebug("ManageRemoteProductHandler: Remote product with ID {ProductId} created successfully.", command.Message.Id);
                }
                else
                {
                    this.logger.LogDebug("ManageRemoteProductHandler: Remote product with ID {ProductId} found. Updating product.", command.Message.Id);
                    product.Id = command.Message.Id;
                    product.Name = command.Message.Name;
                    product.Description = command.Message.Description;
                    product.Price = command.Message.Price;
                    await this.orderRepo.UpdateRemoteProductAsync(product);

                    this.logger.LogDebug("ManageRemoteProductHandler: Remote product with ID {ProductId} updated successfully.", command.Message.Id);
                }

                return CustomHttpResult.Ok(new ManageRemoteProductResult());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ManageRemoteProductHandler: Error managing remote product with ID {ProductId} with message: {message}", command.Message.Id, ex.Message);
                throw;
            }
        }

        protected override CustomResponse<ManageRemoteProductResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<ManageRemoteProductResult>();
        }

        protected override IValidator<ManageRemoteProductCommand> GetValidator()
        {
            var validator = new InlineValidator<ManageRemoteProductCommand>();
            validator.RuleFor(v => v.Message).NotNull();
            validator.RuleFor(v => v.Message.Name).NotEmpty();
            validator.RuleFor(v => v.Message.Description).NotEmpty();
            validator.RuleFor(v => v.Message.Price).NotNull();
            validator.RuleFor(v => v.Message.Id).GreaterThan(0);

            return new InlineValidator<ManageRemoteProductCommand>();
        }
    }
}
