namespace RestaurantSystem.Domain.Exceptions
{
    public class ValidationException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; }

        // ✅ كونستركتور لقاموس الأخطاء (يستخدم عادة مع FluentValidation)
        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", 400)
        {
            Errors = errors;
        }

        // ✅ أضف هذا الكونستركتور الجديد للرسائل البسيطة
        public ValidationException(string message)
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }
    }
}