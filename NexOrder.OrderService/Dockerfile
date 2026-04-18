# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NexOrder.OrderService/NexOrder.OrderService.csproj", "NexOrder.OrderService/"]
COPY ["NexOrder.OrderService.Application/NexOrder.OrderService.Application.csproj", "NexOrder.OrderService.Application/"]
COPY ["NexOrder.OrderService.Domain/NexOrder.OrderService.Domain.csproj", "NexOrder.OrderService.Domain/"]
COPY ["NexOrder.OrderService.Shared/NexOrder.OrderService.Shared.csproj", "NexOrder.OrderService.Shared/"]
COPY ["NexOrder.OrderService.Messages/NexOrder.OrderService.Messages.csproj", "NexOrder.OrderService.Messages/"]
COPY ["NexOrder.OrderService.Infrastructure/NexOrder.OrderService.Infrastructure.csproj", "NexOrder.OrderService.Infrastructure/"]

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

RUN dotnet nuget add source https://nuget.pkg.github.com/$GITHUB_USERNAME/index.json \
    --name github \
    --username $GITHUB_USERNAME \
    --password $GITHUB_TOKEN \
    --store-password-in-clear-text
RUN dotnet restore "./NexOrder.OrderService/NexOrder.OrderService.csproj"
COPY . .
WORKDIR "/src/NexOrder.OrderService"
RUN dotnet build "./NexOrder.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NexOrder.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
 ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true