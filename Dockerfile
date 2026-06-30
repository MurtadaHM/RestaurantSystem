FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RestaurantSystem.sln .
COPY RestaurantSystem.Api/RestaurantSystem.Api.csproj RestaurantSystem.Api/
COPY RestaurantSystem.Application/RestaurantSystem.Application.csproj RestaurantSystem.Application/
COPY RestaurantSystem.Domain/RestaurantSystem.Domain.csproj RestaurantSystem.Domain/
COPY RestaurantSystem.Infrastructure/RestaurantSystem.Infrastructure.csproj RestaurantSystem.Infrastructure/

RUN dotnet restore

COPY . .
WORKDIR /src/RestaurantSystem.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet RestaurantSystem.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
