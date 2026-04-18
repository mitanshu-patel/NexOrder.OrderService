using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NexOrder.Framework.Core.Common;
using NexOrder.Framework.Core.Contracts;
using NexOrder.OrderService.Application.Orders.CreateOrder;
using NexOrder.OrderService.Application.Orders.GetOrder;
using NexOrder.OrderService.Application.Orders.SearchOrders;
using NexOrder.OrderService.Application.Orders.UpdateOrderStatus;
using NexOrder.OrderService.Application.ProductStocks.GetProductStock;
using NexOrder.OrderService.Application.ProductStocks.SaveProductStock;
using NexOrder.OrderService.Application.ProductStocks.SearchProductStocks;
using System.Net;

namespace NexOrder.OrderService;

public class OrderFunctions
{
    private readonly ILogger<OrderFunctions> _logger;
    private readonly IMediator mediator;

    public OrderFunctions(ILogger<OrderFunctions> logger, IMediator mediator)
    {
        _logger = logger;
        this.mediator = mediator;
    }

    [Function("AddProductStock")]
    [OpenApiOperation(operationId: "AddProductStock", tags: new[] { "AddProductStock" }, Description = "Add new product stock.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProductStockCriteria))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SaveProductStockResult))]
    public async Task<IActionResult> AddProductStock([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/product-stocks")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<ProductStockCriteria>(requestBody);
        var result = await this.mediator.SendAsync<SaveProductStockCommand, CustomResponse<SaveProductStockResult>>(new SaveProductStockCommand(data));
        return result.GetResponse();
    }

    [Function("UpdateProductStock")]
    [OpenApiOperation(operationId: "UpdateProductStock", tags: new[] { "UpdateProductStock" }, Description = "Update existing product stock.")]
    [OpenApiParameter(name: "id", Type = typeof(int), Required = true)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProductStockCriteria))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SaveProductStockResult))]
    public async Task<IActionResult> UpdateProductStock([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/product-stocks/{id:int}")] HttpRequest req, int id)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<ProductStockCriteria>(requestBody);
        var result = await this.mediator.SendAsync<SaveProductStockCommand, CustomResponse<SaveProductStockResult>>(new SaveProductStockCommand(data, id));
        return result.GetResponse();
    }

    [Function("GetProductStock")]
    [OpenApiOperation(operationId: "GetProductStock", tags: new[] { "GetProductStock" }, Description = "Get product stock details for given product stock id.")]
    [OpenApiParameter(name: "id", Type = typeof(int), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetProductStockResult))]
    public async Task<IActionResult> GetProductStock([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/product-stocks/{id:int}")] HttpRequest req, int id)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var command = new GetProductStockQuery(id);
        var result = await this.mediator.SendAsync<GetProductStockQuery, CustomResponse<GetProductStockResult>>(command);
        return result.GetResponse();
    }

    [Function("SearchProductStocks")]
    [OpenApiOperation(operationId: "SearchProductStocks", tags: new[] { "SearchProductStocks" }, Description = "Search product stocks for given criteria with pagination.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SearchProductStocksQuery))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SearchProductStocksResult))]
    public async Task<IActionResult> SearchProductStocks([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/product-stocks/search")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<SearchProductStocksQuery>(requestBody);
        var result = await this.mediator.SendAsync<SearchProductStocksQuery, CustomResponse<SearchProductStocksResult>>(data);
        return result.GetResponse();
    }

    [Function("AddNewOrder")]
    [OpenApiOperation(operationId: "AddNewOrder", tags: new[] { "AddNewOrder" }, Description = "Add new order.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(OrderCriteria))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CreateOrderResult))]
    public async Task<IActionResult> AddNewOrder([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/orders")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<OrderCriteria>(requestBody);
        var result = await this.mediator.SendAsync<CreateOrderCommand, CustomResponse<CreateOrderResult>>(new CreateOrderCommand(data));
        return result.GetResponse();
    }

    [Function("UpdateOrderStatus")]
    [OpenApiOperation(operationId: "UpdateOrderStatus", tags: new[] { "UpdateOrderStatus" }, Description = "Update existing order status.")]
    [OpenApiParameter(name: "orderId", Type = typeof(int), Required = true)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(OrderStatusCriteria))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UpdateOrderStatusResult))]
    public async Task<IActionResult> UpdateOrderStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/orders/{orderId:int}/status")] HttpRequest req, int orderId)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<OrderStatusCriteria>(requestBody);
        var result = await this.mediator.SendAsync<UpdateOrderStatusCommand, CustomResponse<UpdateOrderStatusResult>>(new UpdateOrderStatusCommand(orderId, data));
        return result.GetResponse();
    }

    [Function("GetOrderDetails")]
    [OpenApiOperation(operationId: "GetOrderDetails", tags: new[] { "GetOrderDetails" }, Description = "Get order stock details for given order id.")]
    [OpenApiParameter(name: "orderId", Type = typeof(int), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetOrderResult))]
    public async Task<IActionResult> GetOrderDetails([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/orders/{orderId:int}")] HttpRequest req, int orderId)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var command = new GetOrderQuery(orderId);
        var result = await this.mediator.SendAsync<GetOrderQuery, CustomResponse<GetOrderResult>>(command);
        return result.GetResponse();
    }

    [Function("SearchOrders")]
    [OpenApiOperation(operationId: "SearchOrders", tags: new[] { "SearchOrders" }, Description = "Search orders for given criteria with pagination.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SearchOrdersQuery))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SearchOrdersResult))]
    public async Task<IActionResult> SearchOrders([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/orders/search")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<SearchOrdersQuery>(requestBody);
        var result = await this.mediator.SendAsync<SearchOrdersQuery, CustomResponse<SearchOrdersResult>>(data);
        return result.GetResponse();
    }
}