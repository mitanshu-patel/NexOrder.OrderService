using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NexOrder.OrderService
{
    public class OrderIdempotencyMiddleware : IFunctionsWorkerMiddleware
    {
        private Guid parsedKey;
        private readonly List<string> entryPoints;
        public OrderIdempotencyMiddleware(List<string> entryPoints)
        {
            this.entryPoints = entryPoints;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                var functionId = context.FunctionId;
                var shouldRun = this.entryPoints.Contains(context.FunctionDefinition.EntryPoint);
                var idempotencyKey = string.Empty;
                if (shouldRun)
                {
                    idempotencyKey = request.GetIdempotencyKeyValue();
                    var errorMessage = string.Empty;
                    var idempotencyKeyGenerated = true;
                    if (string.IsNullOrEmpty(idempotencyKey))
                    {
                        throw new Exception("Idempotency-Key header is missing");
                    }
                    else
                    {
                        idempotencyKeyGenerated = Guid.TryParse(idempotencyKey, out parsedKey);
                        if (!idempotencyKeyGenerated)
                        {
                            throw new Exception("Idempotency-Key header is incorrect");
                        }
                    }
                }

                await next.Invoke(context);
            }
            catch(ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = JsonSerializer.Serialize(new { Message = ex.Message });
                var response = request!.CreateResponse();
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await response.WriteAsJsonAsync(errorMessage);
                context.GetInvocationResult().Value = response;
            }
        }
    }
}
