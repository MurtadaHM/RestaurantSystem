using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantSystem.Api.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // تسجيل الطلب القادم
            _logger.LogInformation("📩 Incoming Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            // بدء مؤقت لحساب سرعة النظام
            var stopwatch = Stopwatch.StartNew();

            // تمرير الطلب للنظام
            await _next(context);

            stopwatch.Stop();

            // تسجيل الرد الخارج مع الوقت المستغرق
            _logger.LogInformation("📤 Outgoing Response: {StatusCode} in {ElapsedMilliseconds}ms",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
