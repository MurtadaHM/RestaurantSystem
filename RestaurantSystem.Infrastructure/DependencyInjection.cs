using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using RestaurantSystem.Application.Configurations;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Infrastructure.BackgroundServices;
using RestaurantSystem.Infrastructure.Data;
using RestaurantSystem.Infrastructure.ExternalServices.Ai;
using RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy;
using RestaurantSystem.Infrastructure.ExternalServices.Integrations.Team6;
using RestaurantSystem.Infrastructure.Repositories.Implementations;
using System.Net;

namespace RestaurantSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                        typeof(ApplicationDbContext).Assembly.FullName)
                );
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();

            services.AddHttpClient<IAiDiagnosticService, AiDiagnosticService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.Configure<SendyClientSettings>(
                configuration.GetSection("SendyConfig"));

            services.Configure<Team6IntegrationSettings>(
                configuration.GetSection("Team6Integration"));

            services.AddHttpClient<ISendyIntegrationService, SendyIntegrationService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(GetSendyRetryPolicy());

            services.AddHttpClient<ITeam6IntegrationService, Team6IntegrationService>(client =>
            {
                var baseUrl = configuration["Team6Integration:BaseUrl"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("Team6Integration:BaseUrl is missing from configuration.");

                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddHostedService<ReservationStatusWorker>();
            services.AddHostedService<Team6OrderSyncWorker>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetSendyRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                    });
        }
    }
}