using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.OrderService.Application.Common;
using NexOrder.OrderService.Application.Products.ManageRemoteProduct;
using NexOrder.OrderService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NexOrder.OrderService.Application.Users.ManageRemoteUser
{
    public class ManageRemoteUserHandler : RequestHandlerBase<ManageRemoteUserCommand, CustomResponse<ManageRemoteUserResult>>
    {
        private readonly ILogger<ManageRemoteUserHandler> logger;
        private readonly IOrderRepo orderRepo;

        public ManageRemoteUserHandler(ILogger<ManageRemoteUserHandler> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            this.orderRepo = orderRepo;
        }

        protected async override Task<CustomResponse<ManageRemoteUserResult>> ExecuteCommandAsync(ManageRemoteUserCommand command)
        {
            try
            {
                this.logger.LogDebug("ManageRemoteUserHandler: Managing remote user with ID {userId}", command.Message.Id);
                var user = await this.orderRepo.GetRemoteUsers().Where(v => v.Id == command.Message.Id).FirstOrDefaultAsync();
                if (user == null)
                {
                    this.logger.LogDebug("ManageRemoteUserHandler: Remote user with ID {userId} not found. Creating new user.", command.Message.Id);

                    await this.orderRepo.AddRemoteUserAsync(new Domain.Entities.RemoteUser
                    {
                        Id = command.Message.Id,
                        Name = command.Message.FullName,
                        Email = command.Message.Email,
                    });

                    this.logger.LogDebug("ManageRemoteUserHandler: Remote user with ID {userId} created successfully.", command.Message.Id);
                }
                else
                {
                    this.logger.LogDebug("ManageRemoteUserHandler: Remote user with ID {userId} found. Updating user.", command.Message.Id);
                    user.Id = command.Message.Id;
                    user.Name = command.Message.FullName;
                    user.Email = command.Message.Email;
                    await this.orderRepo.UpdateRemoteUserAsync(user);

                    this.logger.LogDebug("ManageRemoteUserHandler: Remote user with ID {userId} updated successfully.", command.Message.Id);
                }

                return CustomHttpResult.Ok(new ManageRemoteUserResult());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ManageRemoteProductHandler: Error managing remote product with ID {ProductId} with message: {message}", command.Message.Id, ex.Message);
                throw;
            }
        }

        protected override CustomResponse<ManageRemoteUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<ManageRemoteUserResult>();
        }

        protected override IValidator<ManageRemoteUserCommand> GetValidator()
        {
            var validator = new InlineValidator<ManageRemoteUserCommand>();
            validator.RuleFor(v => v.Message).NotNull();
            validator.RuleFor(v => v.Message.Id).GreaterThan(0);
            validator.RuleFor(v => v.Message.FullName).NotEmpty();
            validator.RuleFor(v => v.Message.Email).NotEmpty();
            return validator;
        }
    }
}
