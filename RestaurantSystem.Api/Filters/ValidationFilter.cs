using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestaurantSystem.Api.Common;

namespace RestaurantSystem.Api.Filters
{
    /// <summary>
    /// فلتر يقوم بالتحقق من صحة البيانات تلقائياً قبل وصول الطلب للمتحكم
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // التحقق إذا كان هناك أخطاء في الـ ModelState (التي يملؤها FluentValidation)
            if (!context.ModelState.IsValid)
            {
                // استخراج الأخطاء وتحويلها إلى القالب الموحد Dictionary<string, string[]>
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                // تجهيز الرد الموحد باستخدام ApiResponse
                var errorResponse = ApiResponse<object>.Fail("خطأ في التحقق من البيانات المرسلة", errorsInModelState);

                // إرجاع النتيجة مباشرة بـ StatusCode 400 وتوقف تنفيذ الـ Controller
                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            // إذا كانت البيانات سليمة، ننتقل للخطوة التالية (الـ Controller)
            await next();
        }
    }
}