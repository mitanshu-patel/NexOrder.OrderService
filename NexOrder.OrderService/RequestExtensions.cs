using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using NexOrder.OrderService.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService
{
    public static class RequestExtensions
    {
        public static string GetIdempotencyKeyValue(this HttpRequestData request)
        {
            var headerValues = request.Headers.Where(t => t.Key == OrderServiceConstants.IdempotencyKeyHeader).Select(v => v.Value).FirstOrDefault();
            var idempotencyKey = headerValues?.FirstOrDefault();
            return idempotencyKey ?? string.Empty;
        }

        public static Guid GetIdempotencyKey(this HttpRequest request)
        {
            var headerValues = request.Headers.Where(t => t.Key == OrderServiceConstants.IdempotencyKeyHeader).Select(v => v.Value).FirstOrDefault();
            var idempotencyKey = headerValues.FirstOrDefault();
            return Guid.Parse(idempotencyKey ?? string.Empty);
        }
    }
}
