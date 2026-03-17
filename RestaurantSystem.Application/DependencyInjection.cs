using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Application.Services.Implementations;
using RestaurantSystem.Application.Services;

namespace RestaurantSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1. تسجيل خدمات الهوية والمصادقة
            services.AddScoped<IAuthService, AuthService>();

            // 2. تسجيل خدمات المنيو والفئات
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMenuService, MenuService>();

            // 3. تسجيل خدمات العمليات (الطلبات، الطاولات، المدفوعات)
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // 4. تسجيل خدمة التقارير والداشبورد (التي قمنا ببنائها مؤخراً)
            services.AddScoped<IReportService, ReportService>();

            // ملاحظة: الـ AiDiagnosticService يتم تسجيله عادةً في طبقة الـ Infrastructure 
            // لأنه يعتمد على HttpClient وخوارزميات خارجية.

            return services;
        }
    }
}