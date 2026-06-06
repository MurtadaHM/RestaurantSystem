# ============================================================
# Stage 1: Build
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

ARG CONFIGURATION=Release

WORKDIR /src

COPY ["RestaurantSystem.Api/RestaurantSystem.Api.csproj", "RestaurantSystem.Api/"]
COPY ["RestaurantSystem.Application/RestaurantSystem.Application.csproj", "RestaurantSystem.Application/"]
COPY ["RestaurantSystem.Domain/RestaurantSystem.Domain.csproj", "RestaurantSystem.Domain/"]
COPY ["RestaurantSystem.Infrastructure/RestaurantSystem.Infrastructure.csproj", "RestaurantSystem.Infrastructure/"]

RUN dotnet restore "RestaurantSystem.Api/RestaurantSystem.Api.csproj"

COPY . .

WORKDIR /src/RestaurantSystem.Api

RUN dotnet publish "RestaurantSystem.Api.csproj" \
    -c $CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================================
# Stage 2: Runtime
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet RestaurantSystem.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]