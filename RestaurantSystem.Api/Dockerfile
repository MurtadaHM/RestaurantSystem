# ============================================================
# Multi-Stage Build - Optimized for Production (Linux)
# ============================================================

# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG CONFIGURATION=Release

WORKDIR /src

# Copy project files
COPY ["RestaurantSystem.Api/RestaurantSystem.Api.csproj", "RestaurantSystem.Api/"]
COPY ["RestaurantSystem.Application/RestaurantSystem.Application.csproj", "RestaurantSystem.Application/"]
COPY ["RestaurantSystem.Domain/RestaurantSystem.Domain.csproj", "RestaurantSystem.Domain/"]
COPY ["RestaurantSystem.Infrastructure/RestaurantSystem.Infrastructure.csproj", "RestaurantSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "RestaurantSystem.Api/RestaurantSystem.Api.csproj"

# Copy remaining source code
COPY . .

# Build the application
WORKDIR "/src/RestaurantSystem.Api"
RUN dotnet build "RestaurantSystem.Api.csproj" -c $CONFIGURATION -o /app/build

# ============================================================
# Stage 2: Publish Stage
FROM build AS publish
ARG CONFIGURATION=Release

RUN dotnet publish "RestaurantSystem.Api.csproj" \
    -c $CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ============================================================
# Stage 3: Final Stage (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install runtime dependencies
RUN apk add --no-cache curl

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5183/swagger/index.html || exit 1

# Environment setup
ENV ASPNETCORE_URLS=http://+:${PORT:-5183}
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port (default to 5183)
EXPOSE ${PORT:-5183}

# Run application
ENTRYPOINT ["dotnet", "RestaurantSystem.Api.dll"]
