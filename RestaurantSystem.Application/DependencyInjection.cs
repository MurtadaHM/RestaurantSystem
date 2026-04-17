using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Application.Services.Implementations;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1) Auth & Identity
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserManagementService, UserManagementService>();

            // 2) Menu & Structure
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            // 3) Core Operations
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // 4) Inventory
            services.AddScoped<IInventoryService, InventoryService>();

            // 5) Bookings, Reports, Integrations
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ITeam6TrackingService, Team6TrackingService>();
            services.AddScoped<ITeam6OrderSyncService, Team6OrderSyncService>();

            // 6) AutoMapper
            services.AddAutoMapper(config =>
            {
            }, typeof(DependencyInjection).Assembly);

            // 7) FluentValidation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}