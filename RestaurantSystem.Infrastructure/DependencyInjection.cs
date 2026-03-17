using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ✅ Application Layer - Interfaces & Contracts 
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Services.Interfaces;

// ✅ Infrastructure Layer - Implementation
using RestaurantSystem.Infrastructure.ExternalServices;
using RestaurantSystem.Infrastructure.Data;
using RestaurantSystem.Infrastructure.Repositories.Implementations;

namespace RestaurantSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ==========================================
            // 1. قاعدة البيانات (PostgreSQL)
            // ==========================================
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                        typeof(ApplicationDbContext).Assembly.FullName)
                );
            });

            // ==========================================
            // 2. الـ Repositories (تمت إضافة النواقص هنا)
            // ==========================================
            // التسجيل العام (Generic Repository)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // تسجيل المستودعات المتخصصة
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // ✅ الأسطر المضافة لحل مشكلة الـ CategoryService والـ PaymentService:
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // ==========================================
            // 3. AI Diagnostic Service
            // ==========================================
            services.AddHttpClient<IAiDiagnosticService, AiDiagnosticService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}