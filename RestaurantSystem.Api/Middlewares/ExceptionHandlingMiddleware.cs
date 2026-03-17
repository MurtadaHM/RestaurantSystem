using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.Services.Interfaces; // مسار الذكاء الاصطناعي
using RestaurantSystem.Domain.Exceptions; // ✅ تم إضافة مسار الاستثناءات بناءً على صورتك

namespace RestaurantSystem.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // ✅ دالة واحدة تجمع بين تشغيل الـ Request، التقاط الأخطاء، وتشغيل الذكاء الاصطناعي
        public async Task InvokeAsync(HttpContext context, IAiDiagnosticService aiService)
        {
            try
            {
                // تكمل الـ Request بشكل طبيعي
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1. الذكاء الاصطناعي يحلل الخطأ بالخلفية بدون أن يعطل النظام
                try
                {
                    var diagnostic = await aiService.DiagnoseErrorAsync(ex.Message, ex.StackTrace);
                    Console.WriteLine($"\n🤖 [AI Suggestion]: {diagnostic}\n");
                }
                catch (Exception aiEx)
                {
                    _logger.LogWarning(aiEx, "AI Service failed to diagnose the error.");
                }

                // 2. معالجة الخطأ وإرجاعه للمستخدم بشكل مرتب
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // نحدد نوع الخطأ ونرجع Response مناسب
            var response = exception switch
            {
                NotFoundException ex => new { StatusCode = 404, Response = ApiResponse<object>.Fail(ex.Message) },
                ValidationException ex => new { StatusCode = 400, Response = ApiResponse<object>.Fail(ex.Message, ex.Errors) }, // ✅ الآن سيتعرف عليها
                UnauthorizedException ex => new { StatusCode = 401, Response = ApiResponse<object>.Fail(ex.Message) },
                ForbiddenException ex => new { StatusCode = 403, Response = ApiResponse<object>.Fail(ex.Message) },
                ConflictException ex => new { StatusCode = 409, Response = ApiResponse<object>.Fail(ex.Message) },
                // أي خطأ غير متوقع
                _ => new { StatusCode = 500, Response = ApiResponse<object>.Fail("An unexpected error occurred. Please try again later.") }
            };

            // لو خطأ 500 نسجله في الـ Logs
            if (response.StatusCode == 500)
                _logger.LogError(exception, "Unexpected error: {Message}", exception.Message);
            else
                _logger.LogWarning("Handled exception: {Message}", exception.Message);

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response.Response);
        }
    }
}
