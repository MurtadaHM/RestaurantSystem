namespace RestaurantSystem.Api.Common
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // كل الـ Responses ترجع بنفس الشكل دائماً
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        // ✅ نجاح مع بيانات
        public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.")
            => new() { Success = true, Message = message, Data = data };

        // ✅ نجاح بدون بيانات (مثل Delete)
        public static ApiResponse<T> Ok(string message)
            => new() { Success = true, Message = message };

        // ❌ فشل
        public static ApiResponse<T> Fail(string message, Dictionary<string, string[]>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }

}
