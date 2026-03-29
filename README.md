# NexOrder.OrderService

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
- **Docker + containerized deployments**

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
- Runs locally via **Azure Functions Core Tools** or **Docker**

---

## 🛠️ Tech Stack

- **.NET 8**
- **Azure Functions**
- **Entity Framework Core**
- **MediatR**
- **Azure SQL**
- **Azure API Management**
- **Azure Service Bus**
- **Docker / Docker Compose**
- **GitHub Actions**

---

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

### 🧾 Message Contracts

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

### 🧠 Message Subscribe Handling

- Two Service Bus triggered Azure Functions handle inbound events:
    - `ProductServiceFunctions`
    - `UserServiceFunctions`

---

## ⚙️ Local Development (without Docker)

### Prerequisites

- .NET SDK 8+
- Azure Functions Core Tools
- SQL Server (local or Azure)
- `dotnet-ef` CLI

### Restore Dependencies

```bash
dotnet restore
```

### ⚙️ Application Configuration

#### appsettings.json

```json
{
  "ConnectionStrings": {
    "SystemDbConnectionString": "<Azure SQL Connection String>",
    "ServiceBusConnectionString": "<Azure Service Bus Connection String>"
  }
}
```

### Apply EF Core Migrations

```bash
dotnet ef database update \
  --project NexOrder.OrderService.Infrastructure \
  --startup-project NexOrder.OrderService.Infrastructure
```

### Run Locally

```bash
func start
```

---

## 🐳 Docker Support

Similar to other NexOrder services, this service supports running locally and deploying via containers.

### Prerequisites

- Docker Desktop (or Docker Engine)
- Docker Compose v2

### 🧱 Dockerfile

A `Dockerfile` is included to build a container image for the service.

Build an image locally:

```bash
docker build -t nexorder-orderservice:local .
```

Run the container (example):

```bash
docker run --rm -p 8080:80 \
  -e ConnectionStrings__SystemDbConnectionString="<connection-string>" \
  -e ConnectionStrings__ServiceBusConnectionString="<servicebus-connection-string>" \
  nexorder-orderservice:local
```

> Note: Actual port bindings and hosting settings depend on how the Function host is configured in the container.
> 

### 🧩 Docker Compose

A `docker-compose.yml` is included to simplify local orchestration.

Start services:

```bash
docker compose up --build
```

Stop services:

```bash
docker compose down
```

### 🔐 Configuration in Containers

For local containers, prefer **environment variables** (or a local `.env` file referenced by Compose) rather than committing secrets.

Common keys:

- `ConnectionStrings__SystemDbConnectionString`
- `ConnectionStrings__ServiceBusConnectionString`

---

## 🚢 Deployment

### GitHub Actions

The service supports two deployment workflows using **GitHub Actions** with Azure:

1. **Standard deployment (without containerization)** — builds and deploys the Function App directly
2. **Containerized deployment** — builds a Docker image, pushes to Azure Container Registry, and deploys to Azure Web App for Containers

> **Currently, only the containerized deployment workflow is enabled.**
> 

### Standard Deployment Workflow (Disabled)

When enabled, this workflow:

- Builds & restores the .NET project
- Applies EF Core migrations (controlled pipeline step)
- Deploys directly to Azure Functions

> API Management instances are recreated on demand for cost optimization in non-production environments.
> 

### 🧊 Containerized Deployment Workflow (Active)

This service is deployed as a container to **Azure Web App for Containers**.

High-level flow:

1. Build the Docker image via GitHub Actions
2. Push image to **Azure Container Registry**
3. Configure Azure Web App for Containers to pull and run the image
4. Provide required configuration via **App Settings** (environment variables)

Recommended App Settings (examples):

- `ConnectionStrings__SystemDbConnectionString`
- `ConnectionStrings__ServiceBusConnectionString`
- Any other runtime configuration used by the Function host

---

## 🔐 Security & Authentication

- Authentication is handled by a dedicated **Auth Service**
- JWT tokens are validated at **Azure API Management**
- Order Service assumes authenticated requests from API-M
- No authentication logic is embedded inside the microservice

---

## 🌐 API Management Integration

- API is added to API Management by referencing the deployed Azure Function App.
- Inbound policy includes CORS configuration.
- `validate-jwt` inbound policy enforced.
- API Management becomes the only entry point for clients consuming this service.

---

## API Endpoints (Sample)

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | /product-stocks/search | Search product stocks |
| GET | /product-stocks/{id} | Get product stock details by ID |
| POST | /product-stocks | Create new product stock |
| PUT | /product-stocks/{id} | Update existing product stock details |
| POST | /orders/search | Search orders |
| GET | /orders/{id} | Get order details by ID |
| POST | /orders | Create new order |
| PUT | /orders/{id}/status | Update order status |

---

## 📌 Notes

- Business functionality is intentionally minimal.
- Focus is on architecture, cloud integration, and scalability.
- Designed to be consumed by any frontend or service.
