# NexOrder.ProductService

NexOrder.OrderService is a backend microservice responsible for **order creation, order lifecycle management, and coordination between products and users** in the NexOrder system.  
It is built using **Azure Functions**, follows **Clean Architecture principles**, and communicates with other services using **Azure Service Bus**.

---

## 🧱 Overview

NexOrder.OrderService is responsible for **order management** within the NexOrder ecosystem. It manages orders and product stocks.
The service intentionally keeps business functionality simple (CRUD) while demonstrating **real-world backend architecture, cloud-native patterns, security, CI/CD, and messaging**.

---

## 🧩 Key Concepts Demonstrated

- Clean Architecture (Domain / Application / Infrastructure)
- Azure Functions (serverless microservice)
- MediatR (CQRS-style command/query separation)
- Entity Framework Core
- Azure SQL Database
- Azure API Management (API Gateway)
- JWT-based authentication (validated at API-M)
- **Azure Service Bus (event-driven messaging)**
- GitHub Actions CI/CD
- Cloud-ready configuration & secrets handling

---

## 📁 Project Structure

```
NexOrder.OrderService
│
├── NexOrder.OrderService (Azure Function App)
│   └── HTTP & Service Bus triggers
│
├── NexOrder.OrderService.Application
│   ├── CQRS Commands & Queries
│   ├── MediatR handlers
│   └── Business rules & validations
│
├── NexOrder.OrderService.Domain
│   ├── Order & OrderItem entities
│   ├── Domain enums & value objects
│   └── Domain contracts
│
├── NexOrder.OrderService.Infrastructure
│   ├── EF Core DbContext & configurations
│   ├── SQL Server integration
│   ├── External service clients
│   └── Database migrations
│
└── NexOrder.OrderService.Messages
    └── Shared order-related event contracts
```

---

## 🚀 Features

- Create, update, and query orders
- Create, update, and query product stocks
- Clean separation of concerns across layers
- Designed for scalability and extensibility
- Secured behind Azure API Management
- Event publication for downstream services

---

## 🛠️ Tech Stack

- **.NET 8**
- **Azure Functions**
- **Entity Framework Core**
- **MediatR**
- **Azure SQL**
- **Azure API Management**
- **Azure Service Bus**
- **GitHub Actions**

---

## 📣 Event-Driven Messaging

NexOrder.ProductService participates in an **event-driven architecture** using **Azure Service Bus** for asynchronous communication between microservices.

## 🚌 Messaging & Eventing (Azure Service Bus)

### Incoming Events

- **Topic:** `productserviceevents`
- **Subscription:** `productserviceorder`

Used to react to product updates that may affect order processing.

- **Topic:** `userserviceevents`
- **Subscription:** `userserviceorder`

Used to react to user updates that may affect order processing.

Event contracts are defined in:
```
NexOrder.OrderService.Messages
```

### 🧾 Message Contract

Message contracts are defined in a dedicated shared library:

```
NexOrder.OrderService.Messages
└── ProductEvents
  └── ProductUpdated
└── UserEvents
  └── UserUpdated
```

Benefits:
- Strongly typed event contracts
- Clear ownership of integration boundaries
- Easy versioning and reuse across services

---

### 🧠 Message Subscribe Handling

- We've two Service Bus Triggered Azure functions defined as ProductServiceFunctions and UserServiceFunctions which handles respective messages
---

## ⚙️ Local Development

### Prerequisites

- .NET SDK 8+
- Azure Functions Core Tools
- SQL Server (local or Azure)
- dotnet-ef CLI

---

### Restore Dependencies

```bash
dotnet restore
```

---

## ⚙️ Application Configuration

### appsettings.json

``` json
{
  "ConnectionStrings": {
    "SystemDbConnectionString": "<Azure SQL Connection String>",
    "ServiceBusConnectionString": "<Azure Service Bus Connection String>",
  }
}
```

### Apply EF Core Migrations

```bash
dotnet ef database update \
  --project NexOrder.OrderService.Infrastructure \
  --startup-project NexOrder.OrderService.Infrastructure
```

---

### Run Locally

```bash
func start
```

---

## 🔐 Security & Authentication

- Authentication is handled by a dedicated **Auth Service**
- JWT tokens are validated at **Azure API Management**
- Product Service assumes authenticated requests from API-M
- No authentication logic is embedded inside the microservice

---

------------------------------------------------------------------------

## 🌐 API Management Integration

- API is added to API Management by referencing the deployed Azure Function App.
- Inbound policy includes CORS configuration.
- `validate-jwt` inbound policy enforced
- API Management becomes the only entry point for clients consuming this authentication service.

------------------------------------------------------------------------

## API Endpoints (Sample)

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /product-stocks/search | Search product stocks |
| GET | /product-stocks/{id} | Get product stock details by ID |
| POST | /product-stocks | Create new product stock |
| PUT | /product-stocks/{id} | Update existing product stock details |
| POST | /orders/search | Search orders |
| GET | /orders/{id} | Get order details by ID |
| POST | /orders | Create new order |
| PUT | /orders/{id}/status | Update order status |

---

## 🚢 Deployment

The service is deployed using **GitHub Actions** and Azure services:

- Build & restore
- Apply EF Core migrations (controlled pipeline step)
- Deploy to Azure Function App
- Secured and exposed via Azure API Management

> API Management instances are recreated on demand for cost optimization in non-production environments.

---

## 📌 Notes

- Business functionality is intentionally minimal
- Focus is on architecture, cloud integration, and scalability
- Designed to be consumed by any frontend or service

---
